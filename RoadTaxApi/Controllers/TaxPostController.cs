using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadTaxApi.Models;
using RoadTaxApi.Utilities;

namespace RoadTaxApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxPostController : ControllerBase
    {
        private readonly TaxPostContext _taxPostContext;

        public TaxPostController(TaxPostContext context) => _taxPostContext = context;

        // GET: api/TaxPost
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxPost>>> GetTaxPosts()
        {
            return await _taxPostContext.TaxPosts.ToListAsync();
        }

        // GET: api/TaxPost/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaxPost>> GetTaxPost(long id)
        {
            var taxPost = await _taxPostContext.TaxPosts.FindAsync(id);

            if (taxPost == null)
            {
                return NotFound("No tax post found");
            }

            return taxPost;
        }

        // POST: api/TaxPost
        [HttpPost]
        public async Task<ActionResult<TaxPost>> PostTaxPost(TaxPost taxPost)
        {
            //If no value is sent we assume current date and time. 
            if (IsDefaultDateTime(taxPost.Timestamp))
                taxPost.Timestamp = DateTime.Now;

            var calulatedRate = RateCalculator.GetCurrentRate(taxPost.Timestamp);

            if (!ShouldPay(taxPost) || calulatedRate <= 0)
            {
                return Ok("Free of charge");
            }

            if (TaxPostExistsInHour(taxPost))
            {
                UpdateHourlyAmount(taxPost, calulatedRate);
                return Ok("Updated tax for current hour");
            }

            taxPost.Fee = calulatedRate;

            _taxPostContext.TaxPosts.Add(taxPost);
            await _taxPostContext.SaveChangesAsync();


            return CreatedAtAction("GetTaxPost", new { id = taxPost.Id }, taxPost);
        }

        #region Helper Methods

        private bool ShouldPay(TaxPost taxPost)
        {
            if (DailySum(taxPost) >= 60 || taxPost.VehicleType != VehicleType.Car || DateTime.UtcNow.Month == 7 || RateCalculator.IsHoliday(taxPost.Timestamp))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool TaxPostExistsInHour(TaxPost taxPost)
        {
            return _taxPostContext.TaxPosts.Any(t => t.Timestamp.Hour == taxPost.Timestamp.Hour && t.RegNumber == taxPost.RegNumber);
        }


        public bool IsDefaultDateTime(DateTime dateTime)
        {
            return dateTime == default(DateTime);
        }

        internal void UpdateHourlyAmount(TaxPost newPost, int currentRate)
        {
            try
            {
                var existingPost = _taxPostContext.TaxPosts
                    .Where(t => t.Timestamp.Hour == newPost.Timestamp.Hour && t.RegNumber == newPost.RegNumber).FirstOrDefault();
                if (currentRate >= existingPost.Fee)
                {
                    existingPost.Fee = currentRate;
                    existingPost.Timestamp = newPost.Timestamp;
                    _taxPostContext.Entry(existingPost).State = EntityState.Modified;
                    _taxPostContext.SaveChangesAsync().ConfigureAwait(false);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal int DailySum(TaxPost taxPost)
        {
            int sum = 0;
            var posts = _taxPostContext.TaxPosts
                                   .Where(i => i.RegNumber == taxPost.RegNumber && i.Timestamp.Day == DateTime.Now.Day)
                                   .Select(s => s.Fee).ToList();

            foreach (var amount in posts)
            {
                sum += amount;
            }
            return sum;
        }
    }
    #endregion
}
