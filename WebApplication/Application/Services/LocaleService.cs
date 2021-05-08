﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MobileTracking.Core.Models;
using WebApplication.Infrastructure;

namespace MobileTracking.Core.Application
{
    public class LocaleService : ILocaleService
    {
        private readonly DatabaseContext databaseContext;

        public LocaleService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<Locale>> GetLocales(LocaleQuery query)
        {
            return await this.databaseContext.Locales
                .Include(query.IncludeZones, locale => locale.Zones)
                .Include(query.IncludePositions, locale => locale.Zones!, zones => zones.Positions!)
                .Include(query.IncludePositionsCalibrations, locale => locale.Zones!, zones => zones.Positions!, positions => positions.Calibrations!)
                .Include(query.IncludePositionsData, locale => locale.Zones!, zones => zones.Positions!, positions => positions.PositionData!)
                .ToListAsync();
        }

        public async Task<Locale> FindLocaleById(int localeId)
        {
            return await this.databaseContext.Locales.FindAsync(localeId)
                ?? throw NotFoundException<Locale>.ById(localeId);
        }

        public async Task<List<Locale>> FindLocalesByCoordinates(float latitude, float longitude)
        {
            return await this.databaseContext.Locales
                .OrderBy(locale => Math.Pow(locale.Latitude - latitude, 2) + Math.Pow(locale.Longitude - longitude, 2))
                .Take(5)
                .ToListAsync();
        }

        public async Task<bool> DeleteLocale(int localeId)
        {
            try
            {
                var locale = await this.FindLocaleById(localeId);
                this.databaseContext.Remove(locale);
                await this.databaseContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Locale> CreateLocale(CreateOrUpdateLocaleCommand command)
        {
            if (string.IsNullOrEmpty(command.Name))
            {
                throw new InvalidParametersException("Name", command.Name, "Name can't be null or empty");
            }

            if (!command.Latitude.HasValue)
            {
                throw new InvalidParametersException("Latitude", command.Latitude, "Latitude can't be null");
            }

            if (!command.Longitude.HasValue)
            {
                throw new InvalidParametersException("Longitude", command.Longitude, "Longitude can't be null");
            }

            var locale = new Locale(command.Name, command.Description, command.Latitude.Value, command.Longitude.Value);
            this.databaseContext.Add(locale);
            await this.databaseContext.SaveChangesAsync();

            return locale;
        }

        public async Task<Locale> UpdateLocale(int localeId, CreateOrUpdateLocaleCommand command)
        {
            var locale = await this.FindLocaleById(localeId);
            locale.Name = command.Name ?? locale.Name;
            locale.Description = command.Description ?? locale.Description;
            locale.Latitude = command.Latitude ?? locale.Latitude;
            locale.Longitude = command.Longitude ?? locale.Longitude;

            await this.databaseContext.SaveChangesAsync();

            return locale;
        }
    }
}