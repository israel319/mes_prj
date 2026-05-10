using AppPlusPlus.Application.Common;
using AppPlusPlus.Application.Interfaces.Repositories;
using AppPlusPlus.Domain.Entities.Approvisionnement;
using AppPlusPlus.Domain.Entities.Stock;

namespace AppPlusPlus.Application.Services.Approvisionnement;

public class TransformationService : ITransformationService
{
    private readonly IApproRepository _approRepo;
    private readonly IStockRepository _stockRepo;

    public TransformationService(IApproRepository approRepo, IStockRepository stockRepo)
    {
        _approRepo = approRepo;
        _stockRepo = stockRepo;
    }

    public async Task<List<Transformation>> GetTransformationsByLocalisationsAsync(List<int> localisationIds)
    {
        var results = new List<Transformation>();
        foreach (var locId in localisationIds)
        {
            var transformations = await _approRepo.GetTransformationsByLocalisationAsync(locId);
            results.AddRange(transformations);
        }
        return results;
    }

    public async Task DeleteTransformationAsync(int transformationId)
        => await _approRepo.DeleteTransformationAsync(transformationId);

    public async Task<Transformation?> GetByIdAsync(int id)
        => await _approRepo.GetTransformationByIdAsync(id);

    public async Task<ServiceResult> AddAsync(Transformation transformation)
    {
        // Vérifier que la localisation source est renseignée
        if (!transformation.FromLocalisationId.HasValue)
            return ServiceResult.Fail("La localisation source est obligatoire.");

        // Vérifier le stock source
        var stockSource = await _stockRepo.GetByArticleAndLocalisationAsync(
            transformation.FromArticleId, transformation.FromLocalisationId.Value);

        if (stockSource == null)
            return ServiceResult.Fail("Cet article n'existe pas dans le stock de la localisation source.");

        if (stockSource.Qte < transformation.Qte)
            return ServiceResult.Fail($"Stock insuffisant. Disponible : {stockSource.Qte}, demandé : {transformation.Qte}.");

        // Sauvegarder la transformation
        await _approRepo.AddTransformationAsync(transformation);

        // Soustraire du stock source
        var qteAvantSource = stockSource.Qte;
        stockSource.Qte -= transformation.Qte;
        await _stockRepo.UpdateAsync(stockSource);

        // Mouvement de sortie (source)
        await _stockRepo.AddMouvementAsync(new MouvementStock
        {
            IdArticle = transformation.FromArticleId,
            IdLocalisation = transformation.FromLocalisationId.Value,
            TypeMouvement = "SORTIE",
            Quantite = transformation.Qte,
            QteAvant = qteAvantSource,
            QteApres = stockSource.Qte,
            DateMouvement = transformation.Date,
            TypeDocument = "TRANSFORMATION",
            IdDocument = transformation.TransformationId,
            Reference = $"TRANSFO-{transformation.TransformationId}",
            CreePar = transformation.UserLogin,
            DateCreation = DateTime.Now,
            Observation = transformation.Comment
        });

        // Ajouter au stock destination (si localisation destination renseignée)
        if (transformation.ToLocalisationId.HasValue)
        {
            var qteDestination = transformation.ToQte ?? transformation.Qte;
            var stockDest = await _stockRepo.GetByArticleAndLocalisationAsync(
                transformation.ToArticleId, transformation.ToLocalisationId.Value);

            decimal qteAvantDest = 0;
            if (stockDest != null)
            {
                qteAvantDest = stockDest.Qte;
                stockDest.Qte += qteDestination;
                await _stockRepo.UpdateAsync(stockDest);
            }
            else
            {
                stockDest = new Domain.Entities.Stock.Stock
                {
                    IdArticle = transformation.ToArticleId,
                    IdLocalisation = transformation.ToLocalisationId.Value,
                    Qte = qteDestination,
                    DateSys = DateOnly.FromDateTime(DateTime.Now),
                    UserLogin = transformation.UserLogin
                };
                await _stockRepo.AddAsync(stockDest);
            }

            // Mouvement d'entrée (destination)
            await _stockRepo.AddMouvementAsync(new MouvementStock
            {
                IdArticle = transformation.ToArticleId,
                IdLocalisation = transformation.ToLocalisationId.Value,
                TypeMouvement = "ENTREE",
                Quantite = qteDestination,
                QteAvant = qteAvantDest,
                QteApres = stockDest.Qte,
                DateMouvement = transformation.Date,
                TypeDocument = "TRANSFORMATION",
                IdDocument = transformation.TransformationId,
                Reference = $"TRANSFO-{transformation.TransformationId}",
                CreePar = transformation.UserLogin,
                DateCreation = DateTime.Now,
                Observation = transformation.Comment
            });
        }

        return ServiceResult.Ok("Transformation enregistrée avec succès.");
    }

    public async Task<ServiceResult> UpdateAsync(Transformation transformation)
    {
        // Pour la modification, on met à jour sans re-vérifier le stock
        // (la logique de stock a déjà été appliquée à la création)
        await _approRepo.UpdateTransformationAsync(transformation);
        return ServiceResult.Ok("Transformation modifiée avec succès.");
    }
}
