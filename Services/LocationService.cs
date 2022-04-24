﻿using Microsoft.EntityFrameworkCore;

using Sc3S.Data;
using Sc3S.DTO;
using Sc3S.Entities;
using Sc3S.Exceptions;

namespace Sc3S.Services;
public interface ILocationService
{
    //Task<int> CreateArea(int plantId, AreaCreateDto areaCreateDto);

    //Task<int> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto);

    Task<int> CreatePlant(PlantCreateDto plantCreateDto);

    //Task<int> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto);

    //Task DeleteArea(int areaId);

    //Task DeleteCoordinate(int coordinateId);

    //Task DeletePlant(int plantId);

    //Task DeleteSpace(int spaceId);

    //Task<AreaDto> GetAreaById(int areaId);

    //Task<IEnumerable<AreaDto>> GetAreas();

    //Task<IEnumerable<AreaDto>> GetAreasWithSpaces();

    //Task<CoordinateDto> GetCoordinateByIdWithAssets(int coordinateId);

    //Task<IEnumerable<CoordinateDto>> GetCoordinates();

    //Task<IEnumerable<CoordinateDto>> GetCoordinatesWithAssets();

    Task<PlantDto> GetPlantById(int plantId);

    //Task<IEnumerable<PlantDto>> GetPlants();

    //Task<IEnumerable<PlantDto>> GetPlantsWithAreas();

    //Task<SpaceDto> GetSpaceById(int spaceId);

    //Task<IEnumerable<SpaceDto>> GetSpaces();

    //Task<IEnumerable<SpaceDto>> GetSpacesWithCoordinates();

    //Task MarkDeleteArea(int areaId);

    //Task MarkDeleteCoordinate(int coordinateId);

    //Task MarkDeletePlant(int plantId);

    //Task MarkDeleteSpace(int spaceId);

    //Task UpdateArea(int areaId, AreaUpdateDto areaUpdateDto);

    //Task UpdateCoordinate(int coordinateId, CoordinateUpdateDto coordinateUpdateDto);

    Task UpdatePlant(int plantId, PlantUpdateDto plantUpdateDto);

    //Task UpdateSpace(int spaceId, SpaceUpdateDto spaceUpdateDto);
}

public class LocationService : ILocationService
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<LocationService> _logger;

    public LocationService(IDbContextFactory<Sc3SContext> factory, ILogger<LocationService> logger)
    {
        _factory = factory;
        _logger = logger;
    }
    public async Task<int> CreateArea(int plantId, AreaCreateDto areaCreateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();
     
        // get plant
        var plant = await _context.Plants
            .Include(p => p.Areas)
            .FirstOrDefaultAsync(p => p.PlantId == plantId);
        if (plant is null || plant.IsDeleted)
        {
            throw new NotFoundException("Plant not found");
        }
        if (plant.Areas.Any(a => a.Name.ToLower().Trim() == areaCreateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Area name already exists");
            throw new BadRequestException("Area with this name already exists");
        }

        var area = new Area
        {

            Name = areaCreateDto.Name,
            Description = areaCreateDto.Description,
            PlantId = plant.PlantId,
            IsDeleted = false
        };
        // create area
        plant.Areas.Add(area);

        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Area with id {AreaId} created", area.AreaId);
            return area.AreaId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating area");


            throw new BadRequestException("Error creating area");
        }
    }

    public async Task<int> CreateCoordinate(int spaceId, CoordinateCreateDto coordinateCreateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get space
        var space = await _context.Spaces.Include(s => s.Coordinates).FirstOrDefaultAsync(s => s.SpaceId == spaceId);
        if (space is null || space.IsDeleted)
        {
            throw new NotFoundException("Space not found");
        }
        // validate coordinate name

        if (space.Coordinates.Any(c => c.SpaceId == spaceId && c.Name.ToLower().Trim() == coordinateCreateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Coordinate name already exists");
            throw new BadRequestException("Coordinate name already exists");
        }

        var coordinate = new Coordinate
        {

            Name = coordinateCreateDto.Name,
            Description = coordinateCreateDto.Description,
            SpaceId = spaceId,
            IsDeleted = false
        };
        // create coordinate
        space.Coordinates.Add(coordinate);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Coordinate with id {CoordinateId} created", coordinate.CoordinateId);
            return coordinate.CoordinateId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coordinate");


            throw new BadRequestException("Error creating coordinate");
        }
    }

    public async Task<int> CreatePlant(PlantCreateDto plantCreateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // validate plant name
        var duplicate = await _context.Plants
            .AnyAsync(p => p.Name.ToLower().Trim() == plantCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Plant name already exists");
            throw new BadRequestException("Plant name already exists");
        }

        var plant = new Plant
        {

            Name = plantCreateDto.Name,
            Description = plantCreateDto.Description,
            IsDeleted = false
        };
        // create plant
        _context.Plants.Add(plant);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Plant with id {PlantId} created", plant.PlantId);
            return plant.PlantId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");


            throw new BadRequestException("Error creating plant");
        }
    }

    public async Task<int> CreateSpace(int areaId, SpaceCreateDto spaceCreateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // check if area exists
        var area = await _context.Areas.Include(a => a.Spaces).FirstOrDefaultAsync(a => a.AreaId == areaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area with id {AreaId} not found", areaId);
            throw new BadRequestException("Area not found");
        }
        // validate space name
        if (area.Spaces.Any(s => s.AreaId == areaId && s.Name.ToLower().Trim() == spaceCreateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Space name already exists");
            throw new BadRequestException("Space name already exists");
        }

        var space = new Space
        {

            Name = spaceCreateDto.Name,
            Description = spaceCreateDto.Description,
            AreaId = areaId,
            SpaceType = spaceCreateDto.SpaceType,
            IsDeleted = false
        };
        // create space
        area.Spaces.Add(space);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Space with id {SpaceId} created", space.SpaceId);
            return space.SpaceId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating space");


            throw new BadRequestException("Error creating space");
        }
    }

    public async Task DeleteArea(int areaId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get area
        var area = await _context.Areas.FindAsync(areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        // check if area is marked as deleted
        if (area.IsDeleted == false)
        {
            _logger.LogWarning("Area not marked as deleted");
            throw new BadRequestException("Area not marked as deleted");
        }
        _context.Areas.Remove(area);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Area with id {AreaId} deleted", area.AreaId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area");


            throw new BadRequestException("Error deleting area");
        }
    }

    public async Task DeleteCoordinate(int coordinateId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get coordinate
        var coordinate = await _context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        // check if coordinate is marked as deleted
        if (coordinate.IsDeleted == false)
        {
            _logger.LogWarning("Coordinate not marked as deleted");
            throw new BadRequestException("Coordinate not marked as deleted");
        }
        _context.Coordinates.Remove(coordinate);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Coordinate with id {CoordinateId} deleted", coordinate.CoordinateId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");


            throw new BadRequestException("Error deleting coordinate");
        }
    }

    public async Task DeletePlant(int plantId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get plant
        var plant = await _context.Plants.FindAsync(plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            throw new NotFoundException("Plant not found");
        }
        // check if plant is marked as deleted
        if (plant.IsDeleted == false)
        {
            _logger.LogWarning("Plant not marked as deleted");
            throw new BadRequestException("Plant not marked as deleted");
        }
        _context.Plants.Remove(plant);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Plant with id {PlantId} deleted", plant.PlantId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");


            throw new BadRequestException("Error deleting plant");
        }
    }

    public async Task DeleteSpace(int spaceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get space
        var space = await _context.Spaces.FindAsync(spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        // check if space is marked as deleted
        if (space.IsDeleted == false)
        {
            _logger.LogWarning("Space not marked as deleted");
            throw new BadRequestException("Space not marked as deleted");
        }
        _context.Spaces.Remove(space);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Space with id {SpaceId} deleted", space.SpaceId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting space");


            throw new BadRequestException("Error deleting space");
        }
    }

    public async Task<AreaDto> GetAreaById(int areaId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get area
        var area = await _context.Areas
            .AsNoTracking()
            .Select(a => new AreaDto
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UserId = a.UpdatedBy
            }).FirstOrDefaultAsync(a => a.AreaId == areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        // return area
        _logger.LogInformation("Area with id {AreaId} returned", area.AreaId);
        return area;
    }

    public async Task<IEnumerable<AreaDto>> GetAreas()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get areas
        var areas = await _context.Areas
            .AsNoTracking()
            .Select(a => new AreaDto
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UserId = a.UpdatedBy
            }).ToListAsync();
        if (areas is null)
        {
            _logger.LogWarning("No areas found");
            throw new NotFoundException("No areas found");
        }

        // return areas
        _logger.LogInformation("Areas returned");
        return areas;
    }

    public async Task<IEnumerable<AreaDto>> GetAreasWithSpaces()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get areas
        var areas = await _context.Areas
            .AsNoTracking()
            .Select(a => new AreaDto
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UserId = a.UpdatedBy,
                Spaces = a.Spaces.Select(s => new SpaceDto
                {
                    SpaceId = s.SpaceId,
                    Name = s.Name,
                    Description = s.Description,
                    IsDeleted = s.IsDeleted,
                    UserId = s.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (areas is null)
        {
            _logger.LogWarning("No areas found");
            throw new NotFoundException("No areas found");
        }
        // return areas
        _logger.LogInformation("Areas returned");
        return areas;
    }

    public async Task<CoordinateDto> GetCoordinateByIdWithAssets(int coordinateId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get coordinate
        var coordinate = await _context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateDto
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy,
                Assets = c.Assets.Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UpdatedBy
                }).ToList()
            }).FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        // return coordinate
        _logger.LogInformation("Coordinate with id {CoordinateId} returned", coordinate.CoordinateId);
        return coordinate;
    }

    public async Task<IEnumerable<CoordinateDto>> GetCoordinates()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get coordinates
        var coordinates = await _context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateDto
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy
            }).ToListAsync();
        if (coordinates is null)
        {
            _logger.LogWarning("No coordinates found");
            throw new NotFoundException("No coordinates found");
        }
        // return coordinates
        _logger.LogInformation("Coordinates returned");
        return coordinates;
    }

    public async Task<IEnumerable<CoordinateDto>> GetCoordinatesWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get coordinates
        var coordinates = await _context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateDto
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy,
                Assets = c.Assets.Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (coordinates is null)
        {
            _logger.LogWarning("No coordinates found");
            throw new NotFoundException("No coordinates found");
        }
        // return coordinates
        _logger.LogInformation("Coordinates returned");
        return coordinates;
    }

    public async Task<PlantDto> GetPlantById(int plantId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get plant
        var plant = await _context.Plants
            .AsNoTracking()
            .Select(p => new PlantDto
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UserId = p.UpdatedBy
            }).FirstOrDefaultAsync(p => p.PlantId == plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            throw new NotFoundException("Plant not found");
        }
        // return plant
        _logger.LogInformation("Plant with id {PlantId} returned", plant.PlantId);
        return plant;
    }

    public async Task<IEnumerable<PlantDto>> GetPlants()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get plants
        var plants = await _context.Plants
            .AsNoTracking()
            .Select(p => new PlantDto
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UserId = p.UpdatedBy
            }).ToListAsync();
        if (plants is null)
        {
            _logger.LogWarning("No plants found");
            throw new NotFoundException("No plants found");
        }
        // return plants
        _logger.LogInformation("Plants returned");
        return plants;
    }

    public async Task<IEnumerable<PlantDto>> GetPlantsWithAreas()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get plants
        var plants = await _context.Plants
            .AsNoTracking()
            .Select(p => new PlantDto
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UserId = p.UpdatedBy,
                Areas = p.Areas.Select(a => new AreaDto
                {
                    AreaId = a.AreaId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (plants is null)
        {
            _logger.LogWarning("No plants found");
            throw new NotFoundException("No plants found");
        }
        // return plants
        _logger.LogInformation("Plants returned");
        return plants;
    }

    public async Task<SpaceDto> GetSpaceById(int spaceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get space
        var space = await _context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceDto
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                AreaId = s.AreaId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy
            }).FirstOrDefaultAsync(s => s.SpaceId == spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        // return space
        _logger.LogInformation("Space with id {SpaceId} returned", space.SpaceId);
        return space;
    }

    public async Task<IEnumerable<SpaceDto>> GetSpaces()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get spaces
        var spaces = await _context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceDto
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                AreaId = s.AreaId,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy
            }).ToListAsync();
        if (spaces is null)
        {
            _logger.LogWarning("No spaces found");
            throw new NotFoundException("No spaces found");
        }
        // return spaces
        _logger.LogInformation("Spaces returned");
        return spaces;
    }

    public async Task<IEnumerable<SpaceDto>> GetSpacesWithCoordinates()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get spaces
        var spaces = await _context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceDto
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy,
                Coordinates = s.Coordinates.Select(c => new CoordinateDto
                {
                    CoordinateId = c.CoordinateId,
                    Name = c.Name,
                    Description = c.Description,
                    IsDeleted = c.IsDeleted,
                    UserId = c.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (spaces is null)
        {
            _logger.LogWarning("No spaces found");
            throw new NotFoundException("No spaces found");
        }
        // return spaces
        _logger.LogInformation("Spaces returned");
        return spaces;
    }

    public async Task MarkDeleteArea(int areaId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get area
        var area = await _context.Areas
            .Include(a => a.Spaces)
            .FirstOrDefaultAsync(a => a.AreaId == areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");

            throw new NotFoundException("Area not found");
        }
        if (area.IsDeleted)
        {
            _logger.LogWarning("Area already deleted");
            throw new BadRequestException("Area already deleted");
        }
        // check if area has active spaces
        if (area.Spaces.Any(s => s.IsDeleted == false))
        {
            _logger.LogWarning("Area has active spaces");
            throw new BadRequestException("Area has active spaces");
        }
        // mark area as deleted
        area.IsDeleted = true;
        //update area
        _context.Areas.Update(area);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Area with id {AreaId} marked as deleted", area.AreaId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking area as deleted");


            throw new BadRequestException("Error marking area as deleted");
        }
    }

    public async Task MarkDeleteCoordinate(int coordinateId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get coordinate
        var coordinate = await _context.Coordinates.Include(c => c.Assets)
            .FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        if (coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate already deleted");
            throw new BadRequestException("Coordinate already deleted");
        }
        // check if coordinate has active assets
        if (coordinate.Assets.Any(a => a.IsDeleted == false))
        {
            _logger.LogWarning("Coordinate has active assets");
            throw new BadRequestException("Coordinate has active assets");
        }
        // mark coordinate as deleted
        coordinate.IsDeleted = true;
        //update coordinate
        _context.Coordinates.Update(coordinate);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Coordinate with id {CoordinateId} marked as deleted", coordinate.CoordinateId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking coordinate as deleted");


            throw new BadRequestException("Error marking coordinate as deleted");
        }
    }

    public async Task MarkDeletePlant(int plantId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get plant
        var plant = await _context.Plants.Include(p => p.Areas)
            .FirstOrDefaultAsync(p => p.PlantId == plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            throw new NotFoundException("Plant not found");
        }
        if (plant.IsDeleted)
        {
            _logger.LogWarning("Plant already deleted");
            throw new BadRequestException("Plant already deleted");
        }
        // check if plant has active areas
        if (plant.Areas.Any(a => a.IsDeleted == false))
        {
            _logger.LogWarning("Plant has active areas");
            throw new BadRequestException("Plant has active areas");
        }
        // mark plant as deleted
        plant.IsDeleted = true;
        //update plant
        _context.Plants.Update(plant);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Plant with id {PlantId} marked as deleted", plant.PlantId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking plant as deleted");


            throw new BadRequestException("Error marking plant as deleted");
        }
    }

    public async Task MarkDeleteSpace(int spaceId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get space
        var space = await _context.Spaces.Include(s => s.Coordinates)
            .FirstOrDefaultAsync(s => s.SpaceId == spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        if (space.IsDeleted)
        {
            _logger.LogWarning("Space already deleted");
            throw new BadRequestException("Space already deleted");
        }
        // check if space has active coordinates
        if (space.Coordinates.Any(c => c.IsDeleted == false))
        {
            _logger.LogWarning("Space has active coordinates");
            throw new BadRequestException("Space has active coordinates");
        }
        // mark space as deleted
        space.IsDeleted = true;
        //update space
        _context.Spaces.Update(space);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Space with id {SpaceId} marked as deleted", space.SpaceId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking space as deleted");


            throw new BadRequestException("Error marking space as deleted");
        }
    }

    public async Task UpdateArea(int areaId, AreaUpdateDto areaUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get area
        var area = await _context.Areas
            .FirstOrDefaultAsync(a => a.AreaId == areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            throw new NotFoundException("Area not found");
        }
        // check for duplicate name
        if (await _context.Areas.AnyAsync(a => a.AreaId != areaId && a.PlantId == area.PlantId && a.Name.ToLower().Trim() == areaUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Area with name {Name} already exists", areaUpdateDto.Name);
            throw new BadRequestException("Area with name already exists");
        }
        // update area
        area.Name = areaUpdateDto.Name;
        area.Description = areaUpdateDto.Description;
        area.IsDeleted = false;
        // update area
        _context.Areas.Update(area);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Area with id {AreaId} updated", area.AreaId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating area");


            throw new BadRequestException("Error updating area");
        }
    }

    public async Task UpdateCoordinate(int coordinateId, CoordinateUpdateDto coordinateUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get coordinate
        var coordinate = await _context.Coordinates
            .FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        // check for duplicate name
        if (await _context.Coordinates.AnyAsync(c => c.CoordinateId != coordinateId && c.SpaceId == coordinate.SpaceId && c.Name.ToLower().Trim() == coordinateUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Coordinate with name {Name} already exists", coordinateUpdateDto.Name);
            throw new BadRequestException("Coordinate with name already exists");
        }
        // update coordinate
        coordinate.Name = coordinateUpdateDto.Name;
        coordinate.Description = coordinateUpdateDto.Description;
        coordinate.IsDeleted = false;
        // update coordinate
        _context.Coordinates.Update(coordinate);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Coordinate with id {CoordinateId} updated", coordinate.CoordinateId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating coordinate");


            throw new BadRequestException("Error updating coordinate");
        }
    }

    public async Task UpdatePlant(int plantId, PlantUpdateDto plantUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get plant
        var plant = await _context.Plants
            .FirstOrDefaultAsync(p => p.PlantId == plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            throw new NotFoundException("Plant not found");
        }
        // check for duplicate name
        if (await _context.Plants.AnyAsync(p => p.PlantId != plantId && p.Name.ToLower().Trim() == plantUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Plant with name {Name} already exists", plantUpdateDto.Name);
            throw new BadRequestException("Plant with name already exists");
        }

        // update plant
        plant.Name = plantUpdateDto.Name;
        plant.Description = plantUpdateDto.Description;
        plant.IsDeleted = false;
        // update plant
        _context.Plants.Update(plant);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Plant with id {PlantId} updated", plant.PlantId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating plant");


            throw new BadRequestException("Error updating plant");
        }
    }

    public async Task UpdateSpace(int spaceId, SpaceUpdateDto spaceUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get space
        var space = await _context.Spaces
            .FirstOrDefaultAsync(s => s.SpaceId == spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            throw new NotFoundException("Space not found");
        }
        // check for duplicate name
        if (await _context.Spaces.AnyAsync(s => s.SpaceId != spaceId && s.AreaId == space.AreaId && s.Name.ToLower().Trim() == spaceUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Space with name {Name} already exists", spaceUpdateDto.Name);
            throw new BadRequestException("Space with name already exists");
        }
        // update space
        space.Name = spaceUpdateDto.Name;
        space.Description = spaceUpdateDto.Description;
        space.IsDeleted = false;
        // update space
        _context.Spaces.Update(space);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Space with id {SpaceId} updated", space.SpaceId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating space");


            throw new BadRequestException("Error updating space");
        }
    }
}
