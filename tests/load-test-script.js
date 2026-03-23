// ============================================================================
// Script de test de charge pour KCCMaterialFlow
// INT-026: Load test 50 utilisateurs - Test charge concurrente
// 
// Ce script peut être utilisé avec k6 (https://k6.io/)
// Installation: choco install k6  ou  winget install k6
// Exécution: k6 run load-test-script.js
// ============================================================================

import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';

// Métriques personnalisées
const pageLoadTime = new Trend('page_load_time');
const failedRequests = new Rate('failed_requests');
const successfulLogins = new Counter('successful_logins');
const pagesLoaded = new Counter('pages_loaded');

// Configuration du test
export const options = {
    stages: [
        { duration: '30s', target: 10 },   // Montée progressive à 10 utilisateurs
        { duration: '1m', target: 50 },    // Montée à 50 utilisateurs
        { duration: '2m', target: 50 },    // Maintien à 50 utilisateurs
        { duration: '30s', target: 0 },    // Descente progressive
    ],
    thresholds: {
        'http_req_duration': ['p(95)<2000'],     // 95% des requêtes < 2 secondes
        'failed_requests': ['rate<0.1'],          // Moins de 10% d'erreurs
        'page_load_time': ['p(95)<2000'],         // Temps de chargement page < 2s
    },
};

// URL de base de l'application
const BASE_URL = __ENV.BASE_URL || 'https://localhost:5001';

// Utilisateurs de test (à configurer selon votre environnement)
const TEST_USERS = [
    { login: 'testuser1', password: 'Test123!' },
    { login: 'testuser2', password: 'Test123!' },
    { login: 'testuser3', password: 'Test123!' },
    // Ajouter plus d'utilisateurs selon les besoins
];

// Pages principales à tester
const MAIN_PAGES = [
    '/bons-entree',
    '/bons-sortie',
    '/dashboard',
    '/admin/utilisateurs',
    '/admin/departements',
    '/admin/barrieres',
    '/admin/statuts',
    '/admin/roles',
    '/admin/types-materiels',
    '/admin/parametres',
    '/admin/audit-logs',
];

// Fonction utilitaire pour obtenir un utilisateur aléatoire
function getRandomUser() {
    return TEST_USERS[Math.floor(Math.random() * TEST_USERS.length)];
}

// Fonction utilitaire pour obtenir une page aléatoire
function getRandomPage() {
    return MAIN_PAGES[Math.floor(Math.random() * MAIN_PAGES.length)];
}

// Scénario principal
export default function () {
    const user = getRandomUser();

    // Groupe: Connexion
    group('Login Flow', function () {
        // Charger la page de connexion
        let loginPageResponse = http.get(`${BASE_URL}/login`);
        check(loginPageResponse, {
            'login page loaded': (r) => r.status === 200,
        });

        // Simuler la connexion (adapter selon votre mécanisme d'auth)
        let loginResponse = http.post(`${BASE_URL}/api/auth/login`, JSON.stringify({
            login: user.login,
            password: user.password,
        }), {
            headers: { 'Content-Type': 'application/json' },
        });

        let loginSuccess = check(loginResponse, {
            'login successful': (r) => r.status === 200 || r.status === 302,
        });

        if (loginSuccess) {
            successfulLogins.add(1);
        } else {
            failedRequests.add(1);
        }
    });

    sleep(1);

    // Groupe: Navigation pages principales
    group('Main Pages Navigation', function () {
        for (let i = 0; i < 3; i++) {
            const page = getRandomPage();
            const startTime = Date.now();

            let response = http.get(`${BASE_URL}${page}`, {
                timeout: '10s',
            });

            const loadTime = Date.now() - startTime;
            pageLoadTime.add(loadTime);

            let pageLoaded = check(response, {
                'page loaded successfully': (r) => r.status === 200,
                'page load under 2s': (r) => loadTime < 2000,
            });

            if (pageLoaded) {
                pagesLoaded.add(1);
            } else {
                failedRequests.add(1);
            }

            sleep(Math.random() * 2 + 1); // Pause aléatoire 1-3 secondes
        }
    });

    // Groupe: Opérations de lecture (liste des bons)
    group('Read Operations', function () {
        // Liste des bons d'entrée
        let bonsEntreeResponse = http.get(`${BASE_URL}/api/bons-entree?page=1&pageSize=20`);
        check(bonsEntreeResponse, {
            'bons entree list loaded': (r) => r.status === 200,
        });

        // Liste des bons de sortie
        let bonsSortieResponse = http.get(`${BASE_URL}/api/bons-sortie?page=1&pageSize=20`);
        check(bonsSortieResponse, {
            'bons sortie list loaded': (r) => r.status === 200,
        });

        // Liste des utilisateurs (admin)
        let usersResponse = http.get(`${BASE_URL}/api/admin/utilisateurs?page=1&pageSize=20`);
        check(usersResponse, {
            'users list loaded': (r) => r.status === 200 || r.status === 403, // 403 si pas admin
        });
    });

    sleep(Math.random() * 3 + 2); // Pause aléatoire 2-5 secondes entre sessions
}

// Fonction de setup (exécutée une fois au début)
export function setup() {
    console.log(`Starting load test against ${BASE_URL}`);
    console.log(`Testing with ${TEST_USERS.length} user accounts`);
    console.log(`Testing ${MAIN_PAGES.length} main pages`);

    // Vérifier que l'application est accessible
    let response = http.get(`${BASE_URL}/`);
    if (response.status !== 200 && response.status !== 302) {
        console.error(`Application not accessible! Status: ${response.status}`);
    }

    return { startTime: Date.now() };
}

// Fonction de teardown (exécutée une fois à la fin)
export function teardown(data) {
    const duration = (Date.now() - data.startTime) / 1000;
    console.log(`Load test completed in ${duration} seconds`);
}

// ============================================================================
// Pour exécuter ce test:
// 1. Installer k6: choco install k6
// 2. Modifier BASE_URL et TEST_USERS selon votre environnement
// 3. Exécuter: k6 run load-test-script.js
// 4. Ou avec URL personnalisée: k6 run -e BASE_URL=http://localhost:5000 load-test-script.js
// 
// Résultats attendus (INT-026):
// - 50 utilisateurs concurrents supportés
// - 95% des requêtes < 2 secondes
// - Taux d'erreur < 10%
// ============================================================================
