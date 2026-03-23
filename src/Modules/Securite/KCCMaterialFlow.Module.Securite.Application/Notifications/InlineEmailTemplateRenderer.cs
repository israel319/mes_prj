using System.Text;

namespace KCCMaterialFlow.Module.Securite.Notifications;

/// <summary>
/// SEC-041 à SEC-045: Implémentation simple du rendu de templates email
/// Utilise des templates HTML inline pour éviter les dépendances externes
/// </summary>
public class InlineEmailTemplateRenderer : IEmailTemplateRenderer
{
    public Task<string> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default)
    {
        var html = templateName switch
        {
            "AnomalieDetectee" => RenderAnomalieDetectee(model as AnomalieEmailModel),
            "ScanConforme" => RenderScanConforme(model as ScanConformeEmailModel),
            "PretExpirant" => RenderPretExpirant(model as PretExpirantEmailModel),
            _ => throw new ArgumentException($"Template inconnu: {templateName}")
        };

        return Task.FromResult(html);
    }

    /// <summary>
    /// SEC-041: Template email pour anomalie détectée
    /// </summary>
    private static string RenderAnomalieDetectee(AnomalieEmailModel? model)
    {
        if (model == null) return string.Empty;

        var graviteColor = model.NiveauGravite switch
        {
            "Critique" => "#e31b23",
            "Eleve" => "#ed8936",
            "Moyen" => "#4299e1",
            _ => "#a0aec0"
        };

        var graviteLabel = model.NiveauGravite switch
        {
            "Critique" => "CRITIQUE",
            "Eleve" => "ÉLEVÉE",
            "Moyen" => "MOYENNE",
            _ => "FAIBLE"
        };

        var typeLabel = model.TypeAnomalie switch
        {
            "BarriereNonAutorisee" => "Barrière non autorisée",
            "DocumentExpire" => "Document expiré",
            "DocumentInexistant" => "Document inexistant",
            "ScanDuplique" => "Scan dupliqué",
            "QRCodeInvalide" => "QR Code invalide",
            "ItineraireNonRespectee" => "Itinéraire non respecté",
            "SignalementManuel" => "Signalement manuel",
            _ => model.TypeAnomalie
        };

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html><head><meta charset=\"utf-8\"></head>");
        sb.AppendLine("<body style=\"font-family: 'Segoe UI', Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f7fa;\">");
        sb.AppendLine("<div style=\"max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);\">");

        // Header avec couleur gravité
        sb.AppendLine($"<div style=\"background: {graviteColor}; color: white; padding: 20px; text-align: center;\">");
        sb.AppendLine($"<h1 style=\"margin: 0; font-size: 24px;\">⚠️ ANOMALIE DÉTECTÉE</h1>");
        sb.AppendLine($"<p style=\"margin: 10px 0 0; opacity: 0.9;\">Gravité: {graviteLabel}</p>");
        sb.AppendLine("</div>");

        // Contenu
        sb.AppendLine("<div style=\"padding: 25px;\">");

        // Type et Date
        sb.AppendLine("<table style=\"width: 100%; border-collapse: collapse; margin-bottom: 20px;\">");
        sb.AppendLine("<tr>");
        sb.AppendLine($"<td style=\"padding: 10px; background: #f8fafc; border-radius: 6px;\"><strong>Type:</strong> {typeLabel}</td>");
        sb.AppendLine($"<td style=\"padding: 10px; background: #f8fafc; border-radius: 6px;\"><strong>Date:</strong> {model.DateSignalement:dd/MM/yyyy HH:mm}</td>");
        sb.AppendLine("</tr>");
        sb.AppendLine("</table>");

        // Document concerné
        if (!string.IsNullOrEmpty(model.NumeroReference))
        {
            sb.AppendLine("<div style=\"background: #fff5f5; border-left: 4px solid #e31b23; padding: 15px; margin-bottom: 20px; border-radius: 0 8px 8px 0;\">");
            sb.AppendLine($"<strong>Document concerné:</strong><br>");
            sb.AppendLine($"<span style=\"font-family: monospace; font-size: 18px; color: #e31b23;\">{model.NumeroReference}</span>");
            sb.AppendLine($" <span style=\"background: #e31b23; color: white; padding: 2px 8px; border-radius: 4px; font-size: 12px;\">{model.TypeBon}</span>");
            sb.AppendLine("</div>");
        }

        // Description
        sb.AppendLine("<div style=\"margin-bottom: 20px;\">");
        sb.AppendLine("<strong>Description:</strong>");
        sb.AppendLine($"<p style=\"color: #4a5568; line-height: 1.6; margin: 8px 0;\">{model.Description}</p>");
        sb.AppendLine("</div>");

        // Barrière
        if (!string.IsNullOrEmpty(model.NomBarriere))
        {
            sb.AppendLine("<div style=\"margin-bottom: 20px;\">");
            sb.AppendLine($"<strong>Barrière:</strong> {model.NomBarriere}");
            if (!string.IsNullOrEmpty(model.LocalisationBarriere))
            {
                sb.AppendLine($" <span style=\"color: #718096;\">({model.LocalisationBarriere})</span>");
            }
            sb.AppendLine("</div>");
        }

        // Trajet
        if (!string.IsNullOrEmpty(model.Provenance) || !string.IsNullOrEmpty(model.Destination))
        {
            sb.AppendLine("<div style=\"background: #ebf8ff; padding: 15px; border-radius: 8px; margin-bottom: 20px;\">");
            sb.AppendLine("<strong>Trajet prévu:</strong><br>");
            sb.AppendLine($"<span style=\"font-size: 16px;\">{model.Provenance ?? "N/A"} → {model.Destination ?? "N/A"}</span>");
            sb.AppendLine("</div>");
        }

        // Agent
        if (!string.IsNullOrEmpty(model.AgentNom))
        {
            sb.AppendLine($"<p><strong>Signalé par:</strong> {model.AgentNom} ({model.AgentLogin})</p>");
        }

        // Bouton action
        if (!string.IsNullOrEmpty(model.LinkToAnomalie))
        {
            sb.AppendLine("<div style=\"text-align: center; margin-top: 25px;\">");
            sb.AppendLine($"<a href=\"{model.LinkToAnomalie}\" style=\"display: inline-block; background: {graviteColor}; color: white; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: bold;\">Voir les détails</a>");
            sb.AppendLine("</div>");
        }

        sb.AppendLine("</div>");

        // Footer
        sb.AppendLine("<div style=\"background: #f8fafc; padding: 15px; text-align: center; color: #718096; font-size: 12px;\">");
        sb.AppendLine("<p style=\"margin: 0;\">KCC Material Flow - Module Sécurité</p>");
        sb.AppendLine("<p style=\"margin: 5px 0 0;\">Cet email a été généré automatiquement. Merci de ne pas y répondre.</p>");
        sb.AppendLine("</div>");

        sb.AppendLine("</div></body></html>");

        return sb.ToString();
    }

    /// <summary>
    /// SEC-044: Template email pour scan conforme
    /// </summary>
    private static string RenderScanConforme(ScanConformeEmailModel? model)
    {
        if (model == null) return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html><head><meta charset=\"utf-8\"></head>");
        sb.AppendLine("<body style=\"font-family: 'Segoe UI', Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f7fa;\">");
        sb.AppendLine("<div style=\"max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);\">");

        // Header vert
        sb.AppendLine("<div style=\"background: #48bb78; color: white; padding: 20px; text-align: center;\">");
        sb.AppendLine("<h1 style=\"margin: 0; font-size: 24px;\">✅ PASSAGE CONFIRMÉ</h1>");
        sb.AppendLine($"<p style=\"margin: 10px 0 0; opacity: 0.9;\">Barrière: {model.NomBarriere}</p>");
        sb.AppendLine("</div>");

        // Contenu
        sb.AppendLine("<div style=\"padding: 25px;\">");

        sb.AppendLine($"<p>Bonjour {model.ToNom},</p>");
        sb.AppendLine("<p>Le passage suivant a été confirmé avec succès:</p>");

        // Détails
        sb.AppendLine("<table style=\"width: 100%; border-collapse: collapse; margin: 20px 0;\">");
        sb.AppendLine($"<tr><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\"><strong>Numéro de preuve:</strong></td><td style=\"padding: 8px 0; border-bottom: 1px solid #eee; font-family: monospace;\">{model.NumeroPreuve}</td></tr>");
        sb.AppendLine($"<tr><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\"><strong>Document:</strong></td><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\">{model.NumeroReference} ({model.TypeBon})</td></tr>");
        sb.AppendLine($"<tr><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\"><strong>Date/Heure:</strong></td><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\">{model.DateHeureScan:dd/MM/yyyy HH:mm:ss}</td></tr>");
        sb.AppendLine($"<tr><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\"><strong>Trajet:</strong></td><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\">{model.Provenance} → {model.Destination}</td></tr>");
        sb.AppendLine($"<tr><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\"><strong>Matériels:</strong></td><td style=\"padding: 8px 0; border-bottom: 1px solid #eee;\">{model.NombreMateriels} article(s)</td></tr>");
        sb.AppendLine($"<tr><td style=\"padding: 8px 0;\"><strong>Agent:</strong></td><td style=\"padding: 8px 0;\">{model.AgentNom}</td></tr>");
        sb.AppendLine("</table>");

        sb.AppendLine("</div>");

        // Footer
        sb.AppendLine("<div style=\"background: #f0fff4; padding: 15px; text-align: center; color: #276749; font-size: 12px;\">");
        sb.AppendLine("<p style=\"margin: 0;\">Ce message confirme le bon passage des matériels au point de contrôle.</p>");
        sb.AppendLine("</div>");

        sb.AppendLine("</div></body></html>");

        return sb.ToString();
    }

    /// <summary>
    /// SEC-045: Template email pour prêt expirant
    /// </summary>
    private static string RenderPretExpirant(PretExpirantEmailModel? model)
    {
        if (model == null) return string.Empty;

        var urgenceColor = model.JoursRestants <= 3 ? "#e31b23" : model.JoursRestants <= 5 ? "#ed8936" : "#ecc94b";

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html><head><meta charset=\"utf-8\"></head>");
        sb.AppendLine("<body style=\"font-family: 'Segoe UI', Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f7fa;\">");
        sb.AppendLine("<div style=\"max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);\">");

        // Header
        sb.AppendLine($"<div style=\"background: {urgenceColor}; color: white; padding: 20px; text-align: center;\">");
        sb.AppendLine("<h1 style=\"margin: 0; font-size: 24px;\">⏰ ALERTE PRÊT EXPIRANT</h1>");
        sb.AppendLine($"<p style=\"margin: 10px 0 0; font-size: 20px;\">J-{model.JoursRestants}</p>");
        sb.AppendLine("</div>");

        // Contenu
        sb.AppendLine("<div style=\"padding: 25px;\">");

        sb.AppendLine("<p>Bonjour,</p>");
        sb.AppendLine($"<p>Le prêt suivant arrive à expiration dans <strong>{model.JoursRestants} jour(s)</strong>:</p>");

        // Document
        sb.AppendLine($"<div style=\"background: #fffbeb; border-left: 4px solid {urgenceColor}; padding: 15px; margin: 20px 0; border-radius: 0 8px 8px 0;\">");
        sb.AppendLine($"<strong>Document:</strong> <span style=\"font-family: monospace; font-size: 18px; color: #e31b23;\">{model.NumeroReference}</span>");
        sb.AppendLine($"<br><strong>Type:</strong> {model.TypeBon}");
        sb.AppendLine($"<br><strong>Date d'expiration:</strong> {model.DateExpiration:dddd dd MMMM yyyy}");
        sb.AppendLine("</div>");

        // Demandeur
        sb.AppendLine($"<p><strong>Demandeur:</strong> {model.DemandeurNom}</p>");
        sb.AppendLine($"<p><strong>Département:</strong> {model.DemandeurDepartement}</p>");

        // Matériels
        if (model.Materiels.Any())
        {
            sb.AppendLine("<div style=\"margin: 20px 0;\">");
            sb.AppendLine($"<strong>Matériels concernés ({model.NombreMateriels}):</strong>");
            sb.AppendLine("<table style=\"width: 100%; border-collapse: collapse; margin-top: 10px;\">");
            sb.AppendLine("<tr style=\"background: #f8fafc;\"><th style=\"padding: 8px; text-align: left; border: 1px solid #eee;\">Référence</th><th style=\"padding: 8px; text-align: left; border: 1px solid #eee;\">Désignation</th><th style=\"padding: 8px; text-align: center; border: 1px solid #eee;\">Qté</th></tr>");
            foreach (var mat in model.Materiels.Take(10))
            {
                sb.AppendLine($"<tr><td style=\"padding: 8px; border: 1px solid #eee; font-family: monospace;\">{mat.Reference}</td><td style=\"padding: 8px; border: 1px solid #eee;\">{mat.Designation}</td><td style=\"padding: 8px; border: 1px solid #eee; text-align: center;\">{mat.Quantite}</td></tr>");
            }
            if (model.Materiels.Count > 10)
            {
                sb.AppendLine($"<tr><td colspan=\"3\" style=\"padding: 8px; text-align: center; color: #718096;\">... et {model.Materiels.Count - 10} autres</td></tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");
        }

        // Actions requises
        sb.AppendLine("<div style=\"background: #fff5f5; border: 1px solid #feb2b2; padding: 15px; border-radius: 8px; margin-top: 20px;\">");
        sb.AppendLine("<strong>⚡ Actions requises:</strong>");
        sb.AppendLine("<ul style=\"margin: 10px 0; padding-left: 20px;\">");
        sb.AppendLine("<li>Retourner les matériels avant la date d'expiration</li>");
        sb.AppendLine("<li>Ou demander une prolongation du prêt</li>");
        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        // Bouton
        if (!string.IsNullOrEmpty(model.LinkToBon))
        {
            sb.AppendLine("<div style=\"text-align: center; margin-top: 25px;\">");
            sb.AppendLine($"<a href=\"{model.LinkToBon}\" style=\"display: inline-block; background: #e31b23; color: white; padding: 12px 30px; text-decoration: none; border-radius: 6px; font-weight: bold;\">Voir le détail du prêt</a>");
            sb.AppendLine("</div>");
        }

        sb.AppendLine("</div>");

        // Footer
        sb.AppendLine("<div style=\"background: #f8fafc; padding: 15px; text-align: center; color: #718096; font-size: 12px;\">");
        sb.AppendLine("<p style=\"margin: 0;\">KCC Material Flow - Gestion des Prêts</p>");
        sb.AppendLine("<p style=\"margin: 5px 0 0;\">Cet email a été généré automatiquement.</p>");
        sb.AppendLine("</div>");

        sb.AppendLine("</div></body></html>");

        return sb.ToString();
    }
}
