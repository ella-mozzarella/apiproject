using System;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AddressApi.Models;
using AddressApi.Extensions;
using AddressApi.Helpers;

namespace AddressApi.Controllers
{
    [Route("api/Addresses")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AddressContext _context;

        public AddressController(AddressContext context)
        {
            _context = context;
        }

        // GET: api/Addresses?query=Amsterdam&sortBy=Plaats&isAscending=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses(string query, string sortBy, bool isAscending)
        {
            if (_context.Addresses == null)
                return NotFound();

            IQueryable<Address> AddressQuery = QueryHelpers.CreateSearchQuery(_context.Addresses, query);

            List<Address> AddressList = AddressQuery.OrderBy(sortBy, isAscending).ToList();

            if (AddressList == null)
                return NotFound();

            return await Task.FromResult(AddressList);
        }

        // GET: api/Addresses/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(long id)
        {
            if (_context.Addresses == null)
                return NotFound();

            Address? Address = await _context.Addresses.FindAsync(id);

            if (Address == null)
                return NotFound();

            return Address;
        }

        // GET: api/Addresses/Distance?addressIdA=1&addressIdB=2
        [HttpGet("Distance")]
        public async Task<ActionResult<string>> GetDistance(long addressIdA, long addressIdB)
        {
            if (_context.Addresses == null)
                return NotFound();

            Address? AddressA = await _context.Addresses.FindAsync(addressIdA);
            Address? AddressB = await _context.Addresses.FindAsync(addressIdB);

            if (AddressA == null || AddressB == null)
                return NotFound();

            string? distance = GetDistance(AddressA, AddressB).GetAwaiter().GetResult();

            if (distance == null)
                return NotFound();

            return distance;
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(long id, Address Address)
        {
            if (id != Address.Id)
                return BadRequest();

            _context.Entry(Address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address Address)
        {
            _context.Addresses.Add(Address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddress), new { id = Address.Id }, Address);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(long id)
        {
            if (_context.Addresses == null)
                return NotFound();

            Address? Address = await _context.Addresses.FindAsync(id);
            if (Address == null)
                return NotFound();

            _context.Addresses.Remove(Address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(long id)
        {
            return (_context.Addresses?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<string> GetDistance(Address AddressA, Address AddressB)
        {
            //format addresses
            string addressA = AddressA.Huisnummer + "+" + AddressA.Straat + "+" + AddressA.Plaats + "+" + AddressA.Postcode + "+" + AddressA.Land;
            string addressB = AddressB.Huisnummer + "+" + AddressB.Straat + "+" + AddressB.Plaats + "+" + AddressA.Postcode + "+" + AddressB.Land;

            //create url
            string apiKey = "GOOGLE_API_KEY";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + addressB + "&destinations=" + addressA + "&key=" + apiKey;

            //call API and get xml
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            string xml = await response.Content.ReadAsStringAsync();

            //parse xml and get value of text element in distance element
            XDocument doc = XDocument.Parse(xml);

            //assuming required value is always in the text element under distance element, returns null if not found
            string? distance = doc.Descendants("distance").Where(e => e.Element("text") != null).Select(e => e.Element("text")!.Value).FirstOrDefault();

            return distance!;
        }
    }
}
