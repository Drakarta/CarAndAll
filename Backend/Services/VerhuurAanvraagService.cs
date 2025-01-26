using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;
using Backend.Models;

namespace Backend.Services
{
public class VerhuurAanvraagService
{
    private readonly ApplicationDbContext _context;

    public VerhuurAanvraagService(ApplicationDbContext context)
    {
        _context = context;
    }

  public async Task<List<VerhuurAanvraagDetailsDto>> GetVerhuurAanvragen(VoertuigUserModel model, Guid accountId)
{
    IQueryable<VerhuurAanvraag> query = _context.VerhuurAanvragen.Where(v => v.Account.Id == accountId);

    if (model.maand != "Whole year" && model.jaar == 0)
    {
        int monthNumber = DateTime.ParseExact(model.maand, "MMMM", CultureInfo.InvariantCulture).Month;
        query = query.Where(v => v.Startdatum.Month == monthNumber || v.Einddatum.Month == monthNumber);
    }
    else if (model.maand == "Whole year" && model.jaar != 0)
    {
        query = query.Where(v => v.Startdatum.Year == model.jaar || v.Einddatum.Year == model.jaar);
    }
    else if (model.maand != "Whole year" && model.jaar != 0)
    {
        int monthNumber = DateTime.ParseExact(model.maand, "MMMM", CultureInfo.InvariantCulture).Month;
        query = query.Where(v => (v.Startdatum.Year == model.jaar && v.Startdatum.Month == monthNumber) || (v.Einddatum.Year == model.jaar && v.Einddatum.Month == monthNumber));
    }

    return await query.Include(v => v.Voertuig)
                    .Include(v => v.Account)
                      .Select(a => new VerhuurAanvraagDetailsDto
                      {
                          AanvraagID = a.AanvraagID,
                          Startdatum = a.Startdatum,
                          Einddatum = a.Einddatum,
                          Bestemming = a.Bestemming,
                          Kilometers = a.Kilometers,
                          Status = a.Status,
                          Voertuig = new VoertuigDto
                          {
                              Merk = a.Voertuig.Merk,
                              Type = a.Voertuig.Type,
                              Kenteken = a.Voertuig.Kenteken,
                              Kleur = a.Voertuig.Kleur,
                              Prijs_per_dag = a.Voertuig.Prijs_per_dag
                          },
                          AccountID = a.Account.Id,
                          AccountNaam = a.Account.Naam ?? "Unknown",
                          AccountEmail = a.Account.Email ?? "Unknown"
                      }).ToListAsync();
}
}
}
