﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Supershop.Data.Entities;
using Supershop.Models;

namespace Supershop.Data
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        public CountryRepository(DataContext context) : base(context)
        { }

        public async Task AddCityAsync(CityViewModel model)
        {
            var country = await GetCountryWithCitiesAsync(model.CountryId);
            if (country == null)
            {
                return;
            }

            country.Cities!.Add(new City { Name = model.Name });
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteCityAsync(City city)
        {
            var country = await _context.Countries
                .Where(c => c.Cities!.Any(ci => ci.Id == city.Id))
                .FirstOrDefaultAsync();
            if (country == null)
            {
                return 0;
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return country.Id;
        }

        public async Task<City?> GetCityAsync(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        public IQueryable GetCountriesWithCities()
        {
            return _context.Countries
                .Include(c => c.Cities)
                .OrderBy(c => c.Name);
        }

        public async Task<Country?> GetCountryWithCitiesAsync(int id)
        {
            return await _context.Countries
                .Include(c => c.Cities)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateCityAsync(City city)
        {
            var country = await _context.Countries
                .Where(c => c.Cities!.Any(ci => ci.Id == city.Id)).FirstOrDefaultAsync();
            if (country == null)
            {
                return 0;
            }

            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
            return country.Id;
        }
    }
}
