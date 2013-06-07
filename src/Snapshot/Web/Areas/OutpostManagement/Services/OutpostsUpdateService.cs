using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Core.Domain;
using Core.Persistence;
using Domain;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Models.Parsing.Outpost;
using Web.Security;

namespace Web.Areas.OutpostManagement.Services
{
    public class OutpostsUpdateService : IOutpostsUpdateService
    {
        private IQueryService<Outpost> _queryService;
        private IQueryService<Country> _queryCountry;
        private IQueryService<Region> _queryRegion;
        private IQueryService<District> _queryDistrict;
        private IQueryService<Contact> _queryContact;

        private ISaveOrUpdateCommand<Outpost> _saveOrUpdateCommandOutpost;

        private readonly IContactsUpdateService _contactsUpdateService;

        public OutpostsUpdateService(IContactsUpdateService contactsUpdateService, IQueryService<Outpost> queryservice, IQueryService<Country> queryCountry, 
            IQueryService<Region> queryRegion, IQueryService<District> queryDistrict, ISaveOrUpdateCommand<Outpost> saveOrUpdateCommandOutpost, IQueryService<Contact> queryContact)
        {
            _contactsUpdateService = contactsUpdateService;
            _queryService = queryservice;
            _queryCountry = queryCountry;
            _queryRegion = queryRegion;
            _queryDistrict = queryDistrict;
            _saveOrUpdateCommandOutpost = saveOrUpdateCommandOutpost;
            _queryContact = queryContact;
        }

        private bool TryMapParseOutputAsOutput(UserAndClientIdentity loggedUser, Outpost outpost, IParsedOutpost parsedOutpost)
        {
            outpost.Name = parsedOutpost.Name;
            outpost.IsWarehouse = false;
            outpost.Latitude = "(" + parsedOutpost.Latitude + "," + parsedOutpost.Longitude + ")";
            outpost.Client = loggedUser.Client;
            outpost.ByUser = loggedUser.User;
            outpost.DetailMethod = parsedOutpost.ContactDetail;

            outpost.Country = _queryCountry.Query().Where(it => it.Name == parsedOutpost.Country).FirstOrDefault();
            outpost.Region = _queryRegion.Query().Where(it => it.Name == parsedOutpost.Region && it.Country == outpost.Country).FirstOrDefault();
            outpost.District = _queryDistrict.Query().Where(it => it.Name == parsedOutpost.District && it.Region == outpost.Region).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(outpost.Name) || string.IsNullOrWhiteSpace(outpost.Latitude) || outpost.Country == null || 
                string.IsNullOrWhiteSpace(outpost.DetailMethod) || outpost.Region == null || outpost.District == null) return false;

            return true;
        }

        private bool TryUpdateOutpost(UserAndClientIdentity loggedUser, Outpost existentOutpost, Outpost existentCoordonatesOutpost, IParsedOutpost parsedOutpost)
        {
            var existentContact = _queryContact.Query().Where(c => c.ContactDetail == parsedOutpost.ContactDetail && c.IsMainContact && c.Outpost != existentOutpost).FirstOrDefault();

            if (existentCoordonatesOutpost != null && existentOutpost != existentCoordonatesOutpost || existentContact != null) return false;

            if (TryMapParseOutputAsOutput(loggedUser, existentOutpost, parsedOutpost))
            {
                _saveOrUpdateCommandOutpost.Execute(existentOutpost);

                _contactsUpdateService.ManageOutpostContact(loggedUser, existentOutpost, parsedOutpost);

                return true;
            }

            return false;
        }

        private bool TryAddOutpost(Outpost existentCoordonatesOutpost, UserAndClientIdentity loggedUser, IParsedOutpost parsedOutpost)
        {
            var existentContact = _queryContact.Query().Where(c => c.ContactDetail == parsedOutpost.ContactDetail && c.IsMainContact).FirstOrDefault();

            if (existentCoordonatesOutpost != null || existentContact != null) return false;

            var outpost = new Outpost();

            if (TryMapParseOutputAsOutput(loggedUser, outpost, parsedOutpost))
            {
                _saveOrUpdateCommandOutpost.Execute(outpost);

                _contactsUpdateService.AddContact(loggedUser, outpost, parsedOutpost);

                return true;
            }

            return false;
        }

        public OutpostsUpdateResult ManageParseOutposts(UserAndClientIdentity loggedUser, IOutpostsParseResult parseResult)
        {
            var queryOutposts = _queryService.Query().Where(p => p.Client == loggedUser.Client);
            var invalidOutposts = new List<IParsedOutpost>();

            foreach (var parsedOutpost in parseResult.ParsedOutposts)
            {
                var existentOutpost = queryOutposts.Where(it => it.Name == parsedOutpost.Name && it.District.Name == parsedOutpost.District).FirstOrDefault();
                var existentCoordonatesOutpost = queryOutposts.Where(it => it.Latitude == "(" + parsedOutpost.Longitude + "," + parsedOutpost.Latitude + ")").FirstOrDefault();

                if (existentOutpost != null)
                {
                    if (!TryUpdateOutpost(loggedUser, existentOutpost, existentCoordonatesOutpost, parsedOutpost))
                    {
                        invalidOutposts.Add(parsedOutpost);
                    }
                }
                else
                {
                    if (!TryAddOutpost(existentCoordonatesOutpost, loggedUser, parsedOutpost))
                    {
                        invalidOutposts.Add(parsedOutpost);
                    }
                }
            }

            return new OutpostsUpdateResult
            {
                Success = !invalidOutposts.Any(),
                FailedOutposts = invalidOutposts.Any() ? invalidOutposts : null
            };
        }
    }
}