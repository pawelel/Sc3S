using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Data;
using Sc3S.Entities;
using Sc3S.Helpers;

namespace Sc3S.Services;

public interface ISituationService
{
    Task<ServiceResponse<(int, int)>> CreateAssetSituation(AssetSituationUpdateCommand assetSituationUpdate);

    Task<ServiceResponse<(int, int)>> CreateCategorySituation(CategorySituationUpdateCommand categorySituationUpdate);

    Task<ServiceResponse<(int, int)>> CreateDeviceSituation(DeviceSituationUpdateCommand deviceSituationUpdate);

    Task<ServiceResponse<int>> CreateQuestion(QuestionUpdateCommand questionUpdateDto);

    Task<ServiceResponse<int>> CreateSituation(SituationUpdateCommand situationUpdateDto);

    Task<ServiceResponse<(int, int)>> CreateSituationDetail(SituationDetailUpdateCommand situationDetailUpdate);

    Task<ServiceResponse<(int, int)>> CreateSituationParameter(SituationParameterUpdateCommand situationParameterUpdate);

    Task<ServiceResponse<(int, int)>> CreateSituationQuestion(SituationQuestionUpdateCommand situationQuestionUpdate);

    Task<ServiceResponse> DeleteAssetSituation(int assetId, int situationId);

    Task<ServiceResponse> DeleteCategorySituation(int categoryId, int situationId);

    Task<ServiceResponse> DeleteDeviceSituation(int deviceId, int situationId);

    Task<ServiceResponse> DeleteQuestion(int situationId);

    Task<ServiceResponse> DeleteSituation(int situationId);

    Task<ServiceResponse> DeleteSituationDetail(int situationId, int detailId);

    Task<ServiceResponse> DeleteSituationParameter(int situationId, int parameterId);

    Task<ServiceResponse> DeleteSituationQuestion(int situationId, int questionId);

    Task<ServiceResponse<QuestionQuery>> GetQuestionById(int questionId);

    Task<ServiceResponse<IEnumerable<QuestionQuery>>> GetQuestions();

    Task<ServiceResponse<SituationQuery>> GetSituationById(int situationId);

    Task<ServiceResponse<IEnumerable<SituationQuery>>> GetSituations();

    Task<ServiceResponse<IEnumerable<SituationWithAssetsQuery>>> GetSituationsWithAssets();

    Task<ServiceResponse<IEnumerable<SituationWithCategoriesQuery>>> GetSituationsWithCategories();

    Task<ServiceResponse<IEnumerable<SituationWithQuestionsQuery>>> GetSituationsWithQuestions();

    Task<ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsQuery>>> GetSituationWithAssetsAndDetails();

    Task<ServiceResponse> MarkDeleteAssetSituation(AssetSituationUpdateCommand assetSituationUpdate);

    Task<ServiceResponse> MarkDeleteCategorySituation(CategorySituationUpdateCommand categorySituationUpdate);

    Task<ServiceResponse> MarkDeleteDeviceSituation(DeviceSituationUpdateCommand deviceSituationUpdate);

    Task<ServiceResponse> MarkDeleteQuestion(QuestionUpdateCommand questionUpdate);

    Task<ServiceResponse> MarkDeleteSituation(SituationUpdateCommand situationUpdate);

    Task<ServiceResponse> MarkDeleteSituationDetail(SituationDetailUpdateCommand situationDetailUpdate);

    Task<ServiceResponse> MarkDeleteSituationParameter(SituationParameterUpdateCommand situationParameterUpdate);

    Task<ServiceResponse> MarkDeleteSituationQuestion(SituationQuestionUpdateCommand situationQuestionUpdate);

    Task<ServiceResponse> UpdateAssetSituation(AssetSituationUpdateCommand assetSituationUpdate);

    Task<ServiceResponse> UpdateCategorySituation(CategorySituationUpdateCommand categorySituationUpdate);

    Task<ServiceResponse> UpdateDeviceSituation(DeviceSituationUpdateCommand deviceSituationUpdate);

    Task<ServiceResponse> UpdateQuestion(QuestionUpdateCommand questionUpdateDto);

    Task<ServiceResponse> UpdateSituation(SituationUpdateCommand situationUpdateDto);

    Task<ServiceResponse> UpdateSituationDetail(SituationDetailUpdateCommand situationDetailUpdate);

    Task<ServiceResponse> UpdateSituationParameter(SituationParameterUpdateCommand situationParameterUpdate);

    Task<ServiceResponse> UpdateSituationQuestion(SituationQuestionUpdateCommand situationQuestionUpdate);
}

public class SituationService : ISituationService
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<SituationService> _logger;

    public SituationService(IDbContextFactory<Sc3SContext> factory, ILogger<SituationService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResponse<(int, int)>> CreateAssetSituation(AssetSituationUpdateCommand assetSituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await _context.AssetSituations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssetId == assetSituationUpdate.AssetId && a.SituationId == assetSituationUpdate.SituationId);
        if (assetSituation != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie dla assetu już istnieje");
        var asset = await _context.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssetId == assetSituationUpdate.AssetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zasób nie został znaleziony");
        }
        var situation = await _context.Situations.FirstOrDefaultAsync(a => a.SituationId == assetSituationUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie nie zostało znalezione");
        }
        assetSituation = new AssetSituation
        {
            AssetId = assetSituationUpdate.AssetId,
            SituationId = assetSituationUpdate.SituationId,
            IsDeleted = false,
            CreatedBy = assetSituationUpdate.UpdatedBy,
            UpdatedBy = assetSituationUpdate.UpdatedBy
        };
        _context.Add(assetSituation);
        

        try
        {
            await _context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (assetSituationUpdate.AssetId, assetSituationUpdate.AssetId), "Zdarzenie dla assetu zostało utworzone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assetSituation");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas tworzenia zdarzenia dla assetu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateCategorySituation(CategorySituationUpdateCommand categorySituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var categorySituation = await _context.CategorySituations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.CategoryId == categorySituationUpdate.CategoryId && a.SituationId == categorySituationUpdate.SituationId);
        if (categorySituation != null)
        {
            _logger.LogWarning("CategorySituation already exists");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie dla kategorii już istnieje");
        }
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.CategoryId == categorySituationUpdate.CategoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Kategoria nie została znaleziona");
        }
        var situation = await _context.Situations.FirstOrDefaultAsync(a => a.SituationId == categorySituationUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie nie zostało znalezione");
        }
        categorySituation = new CategorySituation
        {
            CategoryId = categorySituationUpdate.CategoryId,
            SituationId = categorySituationUpdate.SituationId,
            IsDeleted = false,
            CreatedBy = categorySituationUpdate.UpdatedBy,
            UpdatedBy = categorySituationUpdate.UpdatedBy
        };
        _context.Add(categorySituation);
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("CategorySituation created");
            return new ServiceResponse<(int, int)>(true, (categorySituationUpdate.CategoryId, categorySituationUpdate.CategoryId), "Zdarzenie dla kategorii zostało utworzone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating categorySituation");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas tworzenia zdarzenia dla kategorii");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateDeviceSituation(DeviceSituationUpdateCommand deviceSituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var deviceSituation = await _context.DeviceSituations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.DeviceId == deviceSituationUpdate.DeviceId && a.SituationId == deviceSituationUpdate.SituationId);
        if (deviceSituation != null)
        {
            _logger.LogWarning("DeviceSituation already exists");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie dla urządzenia już istnieje");
        }
        var device = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.DeviceId == deviceSituationUpdate.DeviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Urządzenie nie zostało znalezione");
        }
        var situation = await _context.Situations.FirstOrDefaultAsync(a => a.SituationId == deviceSituationUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie nie zostało znalezione");
        }
        deviceSituation = new DeviceSituation
        {
            DeviceId = deviceSituationUpdate.DeviceId,
            SituationId = deviceSituationUpdate.SituationId,
            IsDeleted = false,
            CreatedBy = deviceSituationUpdate.UpdatedBy,
            UpdatedBy = deviceSituationUpdate.UpdatedBy
        };
        _context.Add(deviceSituation);
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("DeviceSituation created");
            return new ServiceResponse<(int, int)>(true, (deviceSituationUpdate.DeviceId, deviceSituationUpdate.DeviceId), "Zdarzenie dla urządzenia zostało utworzone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating deviceSituation");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas tworzenia zdarzenia dla urządzenia");
        }
    }

    public async Task<ServiceResponse<int>> CreateQuestion(QuestionUpdateCommand questionUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var duplicate = await _context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == questionUpdate.Name.ToLower().Trim());
        if (duplicate != null)
        {
            _logger.LogWarning("Question already exists");
            return new ServiceResponse<int>(false, -1, "Pytanie już istnieje");
        }
        var question = new Question
        {
            Name = questionUpdate.Name,
            IsDeleted = false,
            CreatedBy = questionUpdate.UpdatedBy,
            UpdatedBy = questionUpdate.UpdatedBy
        };
        _context.Add(question);
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Question created");
            return new ServiceResponse<int>(true, question.QuestionId, "Pytanie zostało utworzone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia pytania");
        }
    }

    public async Task<ServiceResponse<int>> CreateSituation(SituationUpdateCommand situationUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var duplicate = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim());
        if (duplicate != null)
        {
            _logger.LogWarning("Situation already exists");
            return new ServiceResponse<int>(false, -1, "Zdarzenie już istnieje");
        }
        var situation = new Situation
        {
            Name = situationUpdateDto.Name,
            IsDeleted = false,
            CreatedBy = situationUpdateDto.UpdatedBy,
            UpdatedBy = situationUpdateDto.UpdatedBy
        };
        _context.Add(situation);
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Situation created");
            return new ServiceResponse<int>(true, situation.SituationId, "Zdarzenie zostało utworzone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situation");
            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia zdarzenia");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateSituationDetail(SituationDetailUpdateCommand situationDetailUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var duplicate = await _context.SituationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.SituationId == situationDetailUpdate.SituationId && a.DetailId == situationDetailUpdate.DetailId);
        if (duplicate != null)
        {
            _logger.LogWarning("SituationDetail already exists");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Szczegół zdarzenia już istnieje");
        }
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.SituationId == situationDetailUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie nie zostało znalezione");
        }
        var detail = await _context.Details
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.DetailId == situationDetailUpdate.DetailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Szczegół nie zostało znaleziony");
        }
        var situationDetail = new SituationDetail
        {
            SituationId = situationDetailUpdate.SituationId,
            DetailId = situationDetailUpdate.DetailId,
            IsDeleted = false,
            CreatedBy = situationDetailUpdate.UpdatedBy,
            UpdatedBy = situationDetailUpdate.UpdatedBy
        };
        _context.Add(situationDetail);
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("SituationDetail created");
            return new ServiceResponse<(int, int)>(true, (situationDetailUpdate.SituationId, situationDetailUpdate.DetailId), "Szczegół zdarzenia został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situationDetail");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas tworzenia szczegółu zdarzenia");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateSituationParameter(SituationParameterUpdateCommand situationParameterUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var duplicate = await _context.SituationParameters
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.SituationId == situationParameterUpdate.SituationId && a.ParameterId == situationParameterUpdate.ParameterId);
        if (duplicate != null)
        {
            _logger.LogWarning("SituationParameter already exists");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Parametr zdarzenia już istnieje");
        }
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.SituationId == situationParameterUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie nie zostało znalezione");
        }
        var parameter = await _context.Parameters
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ParameterId == situationParameterUpdate.ParameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Parametr nie został znaleziony");
        }
        var situationParameter = new SituationParameter
        {
            SituationId = situationParameterUpdate.SituationId,
            ParameterId = situationParameterUpdate.ParameterId,
            IsDeleted = false,
            CreatedBy = situationParameterUpdate.UpdatedBy,
            UpdatedBy = situationParameterUpdate.UpdatedBy
        };
        _context.Add(situationParameter);
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("SituationParameter created");
            return new ServiceResponse<(int, int)>(true, (situationParameterUpdate.SituationId, situationParameterUpdate.ParameterId), "Parametr zdarzenia został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situationParameter");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas tworzenia parametru zdarzenia");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateSituationQuestion(SituationQuestionUpdateCommand situationQuestionUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var duplicate = await _context.SituationQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.SituationId == situationQuestionUpdate.SituationId && a.QuestionId == situationQuestionUpdate.QuestionId);
        if (duplicate != null)
        {
            _logger.LogWarning("SituationQuestion already exists");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Pytanie zdarzenia już istnieje");
        }
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.SituationId == situationQuestionUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zdarzenie nie zostało znalezione");
        }
        var question = await _context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.QuestionId == situationQuestionUpdate.QuestionId);
        if (question == null || question.IsDeleted)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Pytanie nie zostało znalezione");
        }
        var situationQuestion = new SituationQuestion
        {
            SituationId = situationQuestionUpdate.SituationId,
            QuestionId = situationQuestionUpdate.QuestionId,
            IsDeleted = false,
            CreatedBy = situationQuestionUpdate.UpdatedBy,
            UpdatedBy = situationQuestionUpdate.UpdatedBy
        };
        _context.Add(situationQuestion);
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("SituationQuestion created");
            return new ServiceResponse<(int, int)>(true, (situationQuestionUpdate.SituationId, situationQuestionUpdate.QuestionId), "Pytanie zdarzenia zostało utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situationQuestion");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas tworzenia pytania zdarzenia");
        }
    }

    public async Task<ServiceResponse> DeleteAssetSituation(int assetId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get asset situation
        var assetSituation = await _context.AssetSituations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssetId == assetId && a.SituationId == situationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("Asset situation not found");
            return new ServiceResponse(false, "Zdarzenie assetu nie zostało znalezione");
        }
        // check if AssetSituation is not marked as deleted
        if (assetSituation.IsDeleted == false)
        {
            _logger.LogWarning("Asset situation is not marked as deleted");
            return new ServiceResponse(false, "Zdarzenie assetu nie zostało oznaczone jako usunięte");
        }
        // delete asset situation
        _context.AssetSituations.Remove(assetSituation);
        

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Asset situation with id {AssetId}, {SituationId} deleted", assetId, situationId);
            return new ServiceResponse(true, "Zdarzenie assetu zostało usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset situation");
            return new ServiceResponse(false, "Błąd podczas usuwania zdarzenia assetu");
        }
    }

    public async Task<ServiceResponse> DeleteCategorySituation(int categoryId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get category situation
        var categorySituation = await _context.CategorySituations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.CategoryId == categoryId && a.SituationId == situationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("Category situation not found");
            return new ServiceResponse(false, "Zdarzenie kategorii nie zostało znalezione");
        }
        // check if CategorySituation is not marked as deleted
        if (categorySituation.IsDeleted == false)
        {
            _logger.LogWarning("Category situation is not marked as deleted");
            return new ServiceResponse(false, "Zdarzenie kategorii nie zostało oznaczone jako usunięte");
        }
        // delete category situation
        _context.CategorySituations.Remove(categorySituation);
        

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Category situation with id {CategoryId}, {SituationId} deleted", categoryId, situationId);
            return new ServiceResponse(true, "Zdarzenie kategorii zostało usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category situation");

            return new ServiceResponse(false, "Błąd podczas usuwania zdarzenia kategorii");
        }
    }

    public async Task<ServiceResponse> DeleteDeviceSituation(int deviceId, int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get device situation
        var deviceSituation = await _context.DeviceSituations
            .AsNoTracking()
            .FirstOrDefaultAsync(ds => ds.DeviceId == deviceId && ds.SituationId == situationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("Device situation not found");
            return new ServiceResponse(false, "Zdarzenie urządzenia nie zostało znalezione");
        }
        // check if DeviceSituation is not marked as deleted
        if (deviceSituation.IsDeleted == false)
        {
            _logger.LogWarning("Device situation is not marked as deleted");
            return new ServiceResponse(false, "Zdarzenie urządzenia nie zostało oznaczone jako usunięte");
        }
        // delete device situation
        _context.DeviceSituations.Remove(deviceSituation);
        

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Device situation with id {DeviceId}, {SituationId}  deleted", deviceId, situationId);
            return new ServiceResponse(true, "Zdarzenie urządzenia zostało usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device situation");
            return new ServiceResponse(false, "Błąd podczas usuwania zdarzenia urządzenia");
        }
    }

    public async Task<ServiceResponse> DeleteQuestion(int questionId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get question
        var question = await _context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse(false, "Pytanie nie zostało znalezione");
        }
        // check if question is marked as deleted
        if (question.IsDeleted == false)
        {
            _logger.LogWarning("Question is not marked as deleted");
            return new ServiceResponse(false, "Pytanie nie zostało oznaczone jako usunięte");
        }
        // delete question
        _context.Questions.Remove(question);
        

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Question with id {QuestionId} deleted", question.QuestionId);
            return new ServiceResponse(true, "Pytanie zostało usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            return new ServiceResponse(false, "Błąd podczas usuwania pytania");
        }
    }

    public async Task<ServiceResponse> DeleteSituation(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situation
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        // check if situation is marked as deleted
        if (situation.IsDeleted == false)
        {
            _logger.LogWarning("Situation is not marked as deleted");
            return new ServiceResponse(false, "Zdarzenie nie zostało oznaczone jako usunięte");
        }
        // delete situation
        _context.Situations.Remove(situation);
        

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation with id {SituationId} deleted", situationId);
            return new ServiceResponse(true, "Zdarzenie zostało usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation");
            return new ServiceResponse(false, "Błąd podczas usuwania zdarzenia");
        }
    }

    public async Task<ServiceResponse> DeleteSituationDetail(int situationId, int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situation detail
        var situationDetail = await _context.SituationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(sd => sd.SituationId == situationId && sd.DetailId == detailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("Situation detail not found");
            return new ServiceResponse(false, "Szczegół zdarzenia nie zostało znalezione");
        }
        // check if situation detail is marked as deleted
        if (situationDetail.IsDeleted == false)
        {
            _logger.LogWarning("Situation detail is not marked as deleted");
            return new ServiceResponse(false, "Szczegół zdarzenia nie został oznaczony jako usunięty");
        }
        // delete situation detail
        _context.SituationDetails.Remove(situationDetail);
        

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation detail with id {SituationId}, {DetailId} deleted", situationId, detailId);
            return new ServiceResponse(true, "Szczegół zdarzenia został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation detail");
            return new ServiceResponse(false, "Błąd podczas usuwania szczegółu zdarzenia");
        }
    }

    public async Task<ServiceResponse> DeleteSituationParameter(int situationId, int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situation parameter
        var situationParameter = await _context.SituationParameters
            .AsNoTracking()
            .FirstOrDefaultAsync(sp => sp.SituationId == situationId && sp.ParameterId == parameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("Situation parameter not found");
            return new ServiceResponse(false, "Parametr zdarzenia nie zostało znalezione");
        }
        // check if situation parameter is marked as deleted
        if (situationParameter.IsDeleted == false)
        {
            _logger.LogWarning("Situation parameter is not marked as deleted");
            return new ServiceResponse(false, "Parametr zdarzenia nie został oznaczony jako usunięty");
        }
        // delete situation parameter
        _context.SituationParameters.Remove(situationParameter);
        

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation parameter with id {SituationId}, {ParameterId} deleted", situationId, parameterId);
            return new ServiceResponse(true, "Parametr zdarzenia został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation parameter");
            return new ServiceResponse(false, "Błąd podczas usuwania parametru zdarzenia");
        }
    }

    public async Task<ServiceResponse> DeleteSituationQuestion(int situationId, int questionId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situation question
        var situationQuestion = await _context.SituationQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(sq => sq.SituationId == situationId && sq.QuestionId == questionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("Situation question not found");
            return new ServiceResponse(false, "Pytanie zdarzenia nie zostało znalezione");
        }
        // check if situation question is marked as deleted
        if (situationQuestion.IsDeleted == false)
        {
            _logger.LogWarning("Situation question is not marked as deleted");
            return new ServiceResponse(false, "Pytanie zdarzenia nie zostało oznaczone jako usunięte");
        }
        // delete situation question
        _context.SituationQuestions.Remove(situationQuestion);
        

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            _logger.LogInformation("Situation question with id {SituationId}, {QuestionId} deleted", situationId, questionId);
            return new ServiceResponse(true, "Pytanie zdarzenia zostało usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation question");
            return new ServiceResponse(false, "Błąd podczas usuwania pytania zdarzenia");
        }
    }

    public async Task<ServiceResponse<QuestionQuery>> GetQuestionById(int questionId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get question
        var question = await _context.Questions
            .AsNoTracking()
            .Select(q => new QuestionQuery
            {
                QuestionId = q.QuestionId,
                Name = q.Name,
                IsDeleted = q.IsDeleted,
                UpdatedBy = q.UpdatedBy,
                UpdatedOn = q.UpdatedOn,
                CreatedBy = q.CreatedBy,
                CreatedOn = q.CreatedOn
            })
            .FirstOrDefaultAsync(c => c.QuestionId == questionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse<QuestionQuery>(false, null, "Pytanie nie zostało znalezione");
        }
        // return question
        _logger.LogInformation("Question with id {QuestionId} returned", questionId);
        return new ServiceResponse<QuestionQuery>(true, question, "Pytanie zostało zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<QuestionQuery>>> GetQuestions()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get questions
        var questions = await _context.Questions
            .AsNoTracking()
            .Select(q => new QuestionQuery
            {
                QuestionId = q.QuestionId,
                Name = q.Name,
                IsDeleted = q.IsDeleted,
                UpdatedBy = q.UpdatedBy,
                UpdatedOn = q.UpdatedOn,
                CreatedBy = q.CreatedBy,
                CreatedOn = q.CreatedOn
            })
            .ToListAsync();
        if (questions is null)
        {
            _logger.LogWarning("Questions not found");
            return new ServiceResponse<IEnumerable<QuestionQuery>>(false, null, "Pytania nie zostały znalezione");
        }
        // return questions
        _logger.LogInformation("Questions returned");
        return new ServiceResponse<IEnumerable<QuestionQuery>>(true, questions, "Pytania zostały zwrócone");
    }

    public async Task<ServiceResponse<SituationQuery>> GetSituationById(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situation
        var situation = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationQuery
            {
                SituationId = s.SituationId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                UpdatedOn = s.UpdatedOn,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn
            })
            .FirstOrDefaultAsync(c => c.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse<SituationQuery>(false, null, "Zdarzenie nie zostało znalezione");
        }
        // return situation
        _logger.LogInformation("Situation with id {SituationId} returned", situationId);
        return new ServiceResponse<SituationQuery>(true, situation, "Zdarzenie zostało zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<SituationQuery>>> GetSituations()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situations
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationQuery
            {
                SituationId = s.SituationId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                UpdatedOn = s.UpdatedOn,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn
            })
            .ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            return new ServiceResponse<IEnumerable<SituationQuery>>(false, null, "Zdarzenia nie zostały znalezione");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return new ServiceResponse<IEnumerable<SituationQuery>>(true, situations, "Zdarzenia zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithAssetsQuery>>> GetSituationsWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situations with assets
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithAssetsQuery
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                UpdatedOn = s.UpdatedOn,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn,
                Assets = s.AssetSituations.Select(a => new AssetQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UpdatedBy = a.Asset.UpdatedBy,
                    UpdatedOn = a.Asset.UpdatedOn,
                    CreatedBy = a.Asset.CreatedBy,
                    CreatedOn = a.Asset.CreatedOn
                }).ToList()
            }).ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            return new ServiceResponse<IEnumerable<SituationWithAssetsQuery>>(false, null, "Zdarzenia nie zostały znalezione");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return new ServiceResponse<IEnumerable<SituationWithAssetsQuery>>(true, situations, "Zdarzenia zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithCategoriesQuery>>> GetSituationsWithCategories()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situations with categories
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithCategoriesQuery
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                UpdatedOn = s.UpdatedOn,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn,
                Categories = s.CategorySituations.Select(c => new CategoryQuery
                {
                    CategoryId = c.CategoryId,
                    Name = c.Category.Name,
                    Description = c.Category.Description,
                    IsDeleted = c.Category.IsDeleted,
                    UpdatedBy = c.Category.UpdatedBy,
                    UpdatedOn = c.Category.UpdatedOn,
                    CreatedBy = c.Category.CreatedBy,
                    CreatedOn = c.Category.CreatedOn
                }).ToList()
            }).ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            return new ServiceResponse<IEnumerable<SituationWithCategoriesQuery>>(false, null, "Zdarzenia nie zostały znalezione");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return new ServiceResponse<IEnumerable<SituationWithCategoriesQuery>>(true, situations, "Zdarzenia zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithQuestionsQuery>>> GetSituationsWithQuestions()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situations with questions
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithQuestionsQuery
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                UpdatedOn = s.UpdatedOn,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn,
                Questions = s.SituationQuestions.Select(q => new QuestionQuery
                {
                    QuestionId = q.QuestionId,
                    IsDeleted = q.Question.IsDeleted,
                    UpdatedBy = q.Question.UpdatedBy,
                    UpdatedOn = q.Question.UpdatedOn,
                    CreatedBy = q.Question.CreatedBy,
                    CreatedOn = q.Question.CreatedOn
                }).ToList()
            }).ToListAsync();
        if (situations is null)
        {
            _logger.LogWarning("Situations not found");
            return new ServiceResponse<IEnumerable<SituationWithQuestionsQuery>>(false, null, "Zdarzenia nie zostały znalezione");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return new ServiceResponse<IEnumerable<SituationWithQuestionsQuery>>(true, situations, "Zdarzenia zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsQuery>>> GetSituationWithAssetsAndDetails()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situations with assets and details
        var situations = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithAssetsAndDetailsQuery
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                UpdatedOn = s.UpdatedOn,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn,
                Assets = s.AssetSituations.Select(a => new AssetWithDetailsDisplayQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UpdatedBy = a.Asset.UpdatedBy,
                    UpdatedOn = a.Asset.UpdatedOn,
                    CreatedBy = a.Asset.CreatedBy,
                    CreatedOn = a.Asset.CreatedOn,
                    Details = a.Asset.AssetDetails.Select(d => new AssetDetailDisplayQuery
                    {
                        Name = d.Detail.Name,
                        Description = d.Detail.Description,
                        IsDeleted = d.Detail.IsDeleted,
                        UpdatedBy = d.Detail.UpdatedBy,
                        UpdatedOn = d.Detail.UpdatedOn,
                        CreatedBy = d.Detail.CreatedBy,
                        CreatedOn = d.Detail.CreatedOn
                    }).ToList()
                }).ToList()
            }).ToListAsync();
        // if (situations is null)
        if (situations is null)
        {
            _logger.LogWarning("Situations with asset details not found");
            return new ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsQuery>>(false, null, "Zdarzenia nie zostały znalezione");
        }
        // return situations
        _logger.LogInformation("Situations with asset details returned");
        return new ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsQuery>>(true, situations, "Zdarzenia zostały zwrócone");
    }

    public async Task<ServiceResponse> MarkDeleteAssetSituation(AssetSituationUpdateCommand assetSituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await _context.AssetSituations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssetId == assetSituationUpdate.AssetId && a.SituationId == assetSituationUpdate.SituationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("AssetSituation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        if (assetSituation.IsDeleted)
        {
            _logger.LogWarning("AssetSituation already marked as deleted");
            return new ServiceResponse(false, "Zdarzenie dla zasobu zostało już oznaczone jako usunięte");
        }
        assetSituation.IsDeleted = true;
        assetSituation.UpdatedBy = assetSituationUpdate.UpdatedBy;
        _context.Update(assetSituation);
        

        try
        {
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetSituation with id {AssetId}, {SituationId} marked as deleted", assetSituationUpdate.AssetId, assetSituationUpdate.SituationId);
            return new ServiceResponse(true, "Zdarzenie dla zasobu zostało oznaczone jako usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetSituation with id {AssetId}, {SituationId} as deleted", assetSituationUpdate.AssetId, assetSituationUpdate.SituationId);
            return new ServiceResponse(false, "Błąd podczas oznaczania zdarzenia jako usunięte");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCategorySituation(CategorySituationUpdateCommand categorySituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await _context.CategorySituations
            .AsNoTracking()
            .FirstOrDefaultAsync(cs => cs.CategoryId == categorySituationUpdate.CategoryId && categorySituationUpdate.SituationId == categorySituationUpdate.SituationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("CategorySituation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        if (categorySituation.IsDeleted)
        {
            _logger.LogWarning("CategorySituation already marked as deleted");
            return new ServiceResponse(false, "Zdarzenie zostało już oznaczone jako usunięte");
        }
        categorySituation.IsDeleted = true;
        categorySituation.UpdatedBy = categorySituationUpdate.UpdatedBy;
        _context.Update(categorySituation);
        

        try
        {
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("CategorySituation with id {CategoryId}, {SituationId} marked as deleted", categorySituationUpdate.CategoryId, categorySituationUpdate.SituationId);
            return new ServiceResponse(true, "Zdarzenie dla kategorii zostało oznaczone jako usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking categorySituation with id {CategoryId}, {SituationId} as deleted", categorySituationUpdate.CategoryId, categorySituationUpdate.SituationId);
            return new ServiceResponse(false, "Błąd podczas oznaczania zdarzenia jako usunięte");
        }
    }

    public async Task<ServiceResponse> MarkDeleteDeviceSituation(DeviceSituationUpdateCommand deviceSituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await _context.DeviceSituations
            .AsNoTracking()
            .FirstOrDefaultAsync(ds => ds.DeviceId == deviceSituationUpdate.DeviceId && ds.SituationId == deviceSituationUpdate.SituationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("DeviceSituation not found");
            return new ServiceResponse(false, "Zdarzenie dla urządzenia nie zostało znalezione");
        }
        if (deviceSituation.IsDeleted)
        {
            _logger.LogWarning("DeviceSituation already marked as deleted");
            return new ServiceResponse(false, "Zdarzenie dla urządzenia zostało już oznaczone jako usunięte");
        }
        deviceSituation.IsDeleted = true;
        deviceSituation.UpdatedBy = deviceSituationUpdate.UpdatedBy;
        _context.Update(deviceSituation);
        

        try
        {
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("DeviceSituation with id {DeviceId}, {SituationId} marked as deleted", deviceSituationUpdate.DeviceId, deviceSituationUpdate.SituationId);
            return new ServiceResponse(true, "Zdarzenie dla urządzenia zostało oznaczone jako usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking deviceSituation with id {DeviceId}, {SituationId} as deleted", deviceSituationUpdate.DeviceId, deviceSituationUpdate.SituationId);
            return new ServiceResponse(false, "Błąd podczas oznaczania zdarzenia dla urządzenia jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteQuestion(QuestionUpdateCommand questionUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get question
        var question = await _context.Questions
            .AsNoTracking()
            .Include(q => q.SituationQuestions)
            .FirstOrDefaultAsync(q => q.QuestionId == questionUpdate.QuestionId);
        // if question not found
        if (question == null)
        {
            _logger.LogWarning("Question with id {QuestionId} not found", questionUpdate.QuestionId);
            return new ServiceResponse(false, "Pytanie nie zostało znalezione");
        }
        // if question is already deleted
        if (question.IsDeleted)
        {
            _logger.LogWarning("Question with id {QuestionId} is already deleted", questionUpdate.QuestionId);
            return new ServiceResponse(false, "Pytanie zostało już oznaczone jako usunięte");
        }
        // check if question has SituationQuestions with IsDeleted = false
        if (question.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Question with id {QuestionId} has SituationQuestions with IsDeleted = false", questionUpdate.QuestionId);
            return new ServiceResponse(false, "Pytanie zawiera zdarzenia, które nie zostały oznaczone jako usunięte");
        }

        // mark question as deleted
        question.IsDeleted = true;
        question.UpdatedBy = questionUpdate.UpdatedBy;
        _context.Questions.Update(question);

        try
        {
            
            await _context.SaveChangesAsync();
            // commit transaction

            // return success
            _logger.LogInformation("Question with id {QuestionId} marked as deleted", questionUpdate.QuestionId);
            return new ServiceResponse(true, "Pytanie zostało oznaczone jako usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking question with id {QuestionId} as deleted", questionUpdate.QuestionId);
            return new ServiceResponse(false, "Błąd podczas oznaczania pytania jako usunięte");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituation(SituationUpdateCommand situationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situation
        var situation = await _context.Situations
            .AsNoTracking()
            .Include(s => s.SituationQuestions)
            .Include(s => s.AssetSituations)
            .Include(s => s.CategorySituations)
            .Include(s => s.DeviceSituations)
            .Include(s => s.SituationDetails)
            .Include(s => s.SituationParameters)
            .FirstOrDefaultAsync(s => s.SituationId == situationUpdate.SituationId);
        // if situation not found
        if (situation == null)
        {
            _logger.LogWarning("Situation with id {SituationId} not found", situationUpdate.SituationId);
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        // if situation is already deleted
        if (situation.IsDeleted)
        {
            _logger.LogWarning("Situation with id {SituationId} is already deleted", situationUpdate.SituationId);
            return new ServiceResponse(false, "Zdarzenie zostało już oznaczone jako usunięte");
        }
        // check if situation has SituationQuestions with IsDeleted = false
        if (situation.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationQuestions with IsDeleted = false", situationUpdate.SituationId);
            return new ServiceResponse(false, "Zdarzenie zawiera pytania, które nie zostały oznaczone jako usunięte");
        }
        // check if situation has SituationDetails with IsDeleted = false
        if (situation.SituationDetails.Any(sd => sd.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationDetails with IsDeleted = false", situationUpdate.SituationId);
            return new ServiceResponse(false, "Zdarzenie zawiera szczegóły, które nie zostały oznaczone jako usunięte");
        }
        // check if situation has SituationParameters with IsDeleted = false
        if (situation.SituationParameters.Any(sp => sp.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationParameters with IsDeleted = false", situationUpdate.SituationId);
            return new ServiceResponse(false, "Zdarzenie zawiera parametry, które nie zostały oznaczone jako usunięte");
        }
        // check if situation has AssetSituations with IsDeleted = false
        if (situation.AssetSituations.Any(asit => asit.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has AssetSituations with IsDeleted = false", situationUpdate.SituationId);
            return new ServiceResponse(false, "Zdarzenie zawiera zasoby, które nie zostały oznaczone jako usunięte");
        }
        // check if situation has CategorySituations with IsDeleted = false
        if (situation.CategorySituations.Any(cs => cs.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has CategorySituations with IsDeleted = false", situationUpdate.SituationId);
            return new ServiceResponse(false, "Zdarzenie zawiera kategorie, które nie zostały oznaczone jako usunięte");
        }
        // check if situation has DeviceSituations with IsDeleted = false
        if (situation.DeviceSituations.Any(ds => ds.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has DeviceSituations with IsDeleted = false", situationUpdate.SituationId);
            return new ServiceResponse(false, "Zdarzenie zawiera urządzenia, które nie zostały oznaczone jako usunięte");
        }
        // mark delete situation
        situation.IsDeleted = true;
        situation.UpdatedBy = situationUpdate.UpdatedBy;
        _context.Update(situation);
        

        try
        {
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Situation with id {SituationId} is marked as deleted", situationUpdate.SituationId);
            return new ServiceResponse(true, "Zdarzenie zostało oznaczone jako usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while marking as deleted situation with id {SituationId}", situationUpdate.SituationId);
            return new ServiceResponse(false, "Błąd podczas oznaczania zdarzenia jako usunięte");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationDetail(SituationDetailUpdateCommand situationDetailUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await _context.SituationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(sd => sd.SituationId == situationDetailUpdate.SituationId && sd.DetailId == situationDetailUpdate.DetailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("SituationDetail not found");
            return new ServiceResponse(false, "Szczegół zdarzenia nie został znaleziony");
        }
        if (situationDetail.IsDeleted)
        {
            _logger.LogWarning("SituationDetail already marked as deleted");
            return new ServiceResponse(false, "Szczegół zdarzenia został już oznaczony jako usunięty");
        }

        situationDetail.IsDeleted = true;
        situationDetail.UpdatedBy = situationDetailUpdate.UpdatedBy;
        _context.Update(situationDetail);


        try
        {
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("SituationDetail with id {SituationId}, {DetailId} marked as deleted", situationDetailUpdate.SituationId, situationDetailUpdate.DetailId);
            return new ServiceResponse(true, "Szczegół zdarzenia został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationDetail with id {SituationId}, {DetailId} as deleted", situationDetailUpdate.SituationId, situationDetailUpdate.DetailId);
            return new ServiceResponse(false, "Błąd podczas oznaczania szczegółu zdarzenia jako usunięte");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationParameter(SituationParameterUpdateCommand situationParameterUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await _context.SituationParameters
            .AsNoTracking()
            .FirstOrDefaultAsync(sp => sp.SituationId == situationParameterUpdate.SituationId && sp.ParameterId == situationParameterUpdate.ParameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("SituationParameter not found");
            return new ServiceResponse(false, "Parametr zdarzenia nie został znaleziony");
        }
        if (situationParameter.IsDeleted)
        {
            _logger.LogWarning("SituationParameter already marked as deleted");
            return new ServiceResponse(false, "Parametr zdarzenia został już oznaczony jako usunięty");
        }
        situationParameter.IsDeleted = true;
        situationParameter.UpdatedBy = situationParameterUpdate.UpdatedBy;
        _context.Update(situationParameter);


        try
        {
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("SituationParameter with id {SituationId}, {ParameterId} marked as deleted", situationParameterUpdate.SituationId, situationParameterUpdate.ParameterId);
            return new ServiceResponse(true, "Parametr zdarzenia został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationParameter with id {SituationId}, {ParameterId} as deleted", situationParameterUpdate.SituationId, situationParameterUpdate.ParameterId);
            return new ServiceResponse(false, "Błąd podczas oznaczania parametru zdarzenia jako usunięte");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationQuestion(SituationQuestionUpdateCommand situationQuestionUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await _context.SituationQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(sq => sq.SituationId == situationQuestionUpdate.SituationId && sq.QuestionId == situationQuestionUpdate.QuestionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("SituationQuestion not found");
            return new ServiceResponse(false, "Pytanie zdarzenia nie zostało znalezione");
        }
        if (situationQuestion.IsDeleted)
        {
            _logger.LogWarning("SituationQuestion already marked as deleted");
            return new ServiceResponse(false, "Pytanie zdarzenia zostało już oznaczone jako usunięte");
        }
        situationQuestion.IsDeleted = true;
        situationQuestion.UpdatedBy = situationQuestionUpdate.UpdatedBy;

        _context.Update(situationQuestion);
        

        try
        {
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("SituationQuestion with id {SituationId}, {QuestionId} marked as deleted", situationQuestionUpdate.SituationId, situationQuestionUpdate.QuestionId);
            return new ServiceResponse(true, "Pytanie zdarzenia zostało oznaczone jako usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationQuestion with id {SituationId}, {QuestionId} as deleted", situationQuestionUpdate.SituationId, situationQuestionUpdate.QuestionId);
            return new ServiceResponse(false, "Błąd podczas oznaczania pytania zdarzenia jako usunięte");
        }
    }

    public async Task<ServiceResponse> UpdateAssetSituation(AssetSituationUpdateCommand assetSituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await _context.AssetSituations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssetId == assetSituationUpdate.AssetId && a.SituationId == assetSituationUpdate.SituationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("AssetSituation not found");
            return new ServiceResponse(false, "Zdarzenie dla zasobu nie zostało znalezione");
        }
        if (!assetSituation.IsDeleted)
        {
            _logger.LogWarning("AssetSituation not marked as deleted");
            return new ServiceResponse(false, "Zdarzenie zasobu nie zostało oznaczone jako usunięte");
        }
        var asset = await _context.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssetId== assetSituationUpdate.AssetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse(false, "Zasób nie został znaleziony");
        }
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(a=>a.SituationId == assetSituationUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        assetSituation.IsDeleted = false;
        assetSituation.UpdatedBy = assetSituationUpdate.UpdatedBy;
        
        _context.Update(assetSituation);

        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Zdarzenie zasobu zostało pomyślnie przywrócone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetSituation");
            return new ServiceResponse(false, "Błąd podczas przywracania zdarzenia zasobu");
        }
    }

    public async Task<ServiceResponse> UpdateCategorySituation(CategorySituationUpdateCommand categorySituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await _context.CategorySituations
            .AsNoTracking()
            .FirstOrDefaultAsync(cs => cs.CategoryId == categorySituationUpdate.CategoryId && cs.SituationId == categorySituationUpdate.SituationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("CategorySituation not found");
            return new ServiceResponse(false, "Zdarzenie dla kategorii nie zostało znalezione");
        }
        if (!categorySituation.IsDeleted)
        {
            _logger.LogWarning("CategorySituation not marked as deleted");
            return new ServiceResponse(false, "Zdarzenie kategorii nie zostało oznaczone jako usunięte");
        }
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c=> c.CategoryId == categorySituationUpdate.CategoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse(false, "Kategoria nie została znaleziona");
        }
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.SituationId == categorySituationUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        categorySituation.IsDeleted = false;
        categorySituation.UpdatedBy = categorySituationUpdate.UpdatedBy;
        
        _context.Update(categorySituation);
        
        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Zdarzenie kategorii zostało pomyślnie przywrócone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating categorySituation");
            return new ServiceResponse(false, "Błąd podczas przywracania zdarzenia kategorii");
        }
    }

    public async Task<ServiceResponse> UpdateDeviceSituation(DeviceSituationUpdateCommand deviceSituationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await _context.DeviceSituations
            .AsNoTracking()
            .FirstOrDefaultAsync(ds => ds.DeviceId == deviceSituationUpdate.DeviceId && ds.SituationId == deviceSituationUpdate.SituationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("DeviceSituation not found");
            return new ServiceResponse(false, "Zdarzenie dla urządzenia nie zostało znalezione");
        }
        if (!deviceSituation.IsDeleted)
        {
            _logger.LogWarning("DeviceSituation not marked as deleted");
            return new ServiceResponse(false, "Zdarzenie urządzenia nie zostało oznaczone jako usunięte");
        }
        var device = await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.DeviceId == deviceSituationUpdate.DeviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse(false, "Urządzenie nie zostało znalezione");
        }
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SituationId == deviceSituationUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        deviceSituation.IsDeleted = false;
        deviceSituation.UpdatedBy = deviceSituationUpdate.UpdatedBy;

        _context.Update(deviceSituation);
        

        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Zdarzenie urządzenia zostało pomyślnie przywrócone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating deviceSituation");
            return new ServiceResponse(false, "Błąd podczas przywracania zdarzenia urządzenia");
        }
    }

    public async Task<ServiceResponse> UpdateQuestion(QuestionUpdateCommand questionUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get question
        var question = await _context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.QuestionId == questionUpdate.QuestionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse(false, "Pytanie nie zostało znalezione");
        }
        // check if question name from dto is already taken
        var duplicate = await _context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == questionUpdate.Name.ToLower().Trim());
        if (duplicate is not null)
        {
            _logger.LogWarning("Question name is already taken");
            return new ServiceResponse(false, "Nazwa pytania jest już zajęta");
        }

        question.Name = questionUpdate.Name;
        // assign userId to update
        question.IsDeleted = false;
        question.UpdatedBy = questionUpdate.UpdatedBy;
        // update question
        _context.Update(question);
        try
        {
            
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Question updated");
            return new ServiceResponse(true, "Pytanie zostało pomyślnie zaktualizowane");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating question");
            return new ServiceResponse(false, "Błąd podczas aktualizacji pytania");
        }
    }

    public async Task<ServiceResponse> UpdateSituation( SituationUpdateCommand situationUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situation
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.SituationId == situationUpdate.SituationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        // check if situation name from dto is already taken
        var duplicate = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == situationUpdate.Name.ToLower().Trim());
        if (duplicate is not null)
        {
            _logger.LogWarning("Situation name is already taken");
            return new ServiceResponse(false, "Nazwa zdarzenia jest już zajęta");
        }

        situation.Name = situationUpdate.Name;
        // assign userId to update
        situation.IsDeleted = false;
        situation.UpdatedBy = situationUpdate.UpdatedBy;
        // update situation
        _context.Update(situation);
        
        try
        {
            
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Situation updated");
            return new ServiceResponse(true, "Zdarzenie zostało pomyślnie zaktualizowane");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating situation");
            return new ServiceResponse(false, "Błąd podczas aktualizacji zdarzenia");
        }
    }

    public async Task<ServiceResponse> UpdateSituationDetail(SituationDetailUpdateCommand situationDetailUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await _context.SituationDetails
                .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SituationId == situationDetailUpdate.SituationId && s.DetailId == situationDetailUpdate.DetailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("SituationDetail not found");
            return new ServiceResponse(false, "Szczegół zdarzenia nie został znaleziony");
        }
        if (!situationDetail.IsDeleted)
        {
            _logger.LogWarning("SituationDetail not marked as deleted");
            return new ServiceResponse(false, "Szczegół zdarzenia nie został oznaczony jako usunięty");
        }
        var situation = await _context.Situations.FirstOrDefaultAsync(c => c.SituationId == situationDetailUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        var detail = await _context.Details.FirstOrDefaultAsync(s => s.DetailId == situationDetailUpdate.DetailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse(false, "Szczegół nie został znaleziony");
        }
        situationDetail.IsDeleted = false;
        situationDetail.UpdatedBy = situationDetailUpdate.UpdatedBy;
        _context.Update(situationDetail);


        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Szczegół zdarzenia został pomyślnie przywrócony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationDetail");
            return new ServiceResponse(false, "Błąd podczas przywracania szczegółu zdarzenia");
        }
    }

    public async Task<ServiceResponse> UpdateSituationParameter(SituationParameterUpdateCommand situationParameterUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await _context.SituationParameters
            .AsNoTracking()
            .FirstOrDefaultAsync(sp => sp.SituationId == situationParameterUpdate.SituationId && sp.ParameterId == situationParameterUpdate.ParameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("SituationParameter not found");
            return new ServiceResponse(false, "Parametr zdarzenia nie został znaleziony");
        }
        if (!situationParameter.IsDeleted)
        {
            _logger.LogWarning("SituationParameter not marked as deleted");
            return new ServiceResponse(false, "Parametr zdarzenia nie został oznaczony jako usunięty");
        }
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(s=>s.SituationId==situationParameterUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        var parameter = await _context.Parameters
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ParameterId == situationParameterUpdate.ParameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse(false, "Parametr nie został znaleziony");
        }
        situationParameter.IsDeleted = false;
        situationParameter.UpdatedBy = situationParameterUpdate.UpdatedBy;
        _context.Update(situationParameter);

        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Parametr zdarzenia został pomyślnie przywrócony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationParameter");
            return new ServiceResponse(false, "Błąd podczas przywracania parametru zdarzenia");
        }
    }

    public async Task<ServiceResponse> UpdateSituationQuestion(SituationQuestionUpdateCommand situationQuestionUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await _context.SituationQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SituationId == situationQuestionUpdate.SituationId && s.QuestionId == situationQuestionUpdate.QuestionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("SituationQuestion not found");
            return new ServiceResponse(false, "Pytanie zdarzenia nie zostało znalezione");
        }
        if (!situationQuestion.IsDeleted)
        {
            _logger.LogWarning("SituationQuestion not marked as deleted");
            return new ServiceResponse(false, "Pytanie zdarzenia nie zostało oznaczone jako usunięte");
        }
        var situation = await _context.Situations
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SituationId == situationQuestionUpdate.SituationId);
        if (situation == null || situation.IsDeleted)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse(false, "Zdarzenie nie zostało znalezione");
        }
        var question = await _context.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.QuestionId == situationQuestionUpdate.QuestionId);
        if (question == null || question.IsDeleted)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse(false, "Pytanie nie zostało znalezione");
        }
        situationQuestion.IsDeleted = false;
        situationQuestion.UpdatedBy = situationQuestionUpdate.UpdatedBy;
        _context.Update(situationQuestion);


        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Pytanie zdarzenia zostało pomyślnie przywrócone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationQuestion");
            return new ServiceResponse(false, "Błąd podczas przywracania pytania zdarzenia");
        }
    }
}