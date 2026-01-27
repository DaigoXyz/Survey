using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Survey.DTOs.Survey;
using Survey.Services.Survey;

namespace Survey.Controllers
{
    [ApiController]
    [Route("api/survey-templates")]
    public class SurveyTemplatesController : ControllerBase
    {
        private readonly ISurveyTemplateService _service;
        public SurveyTemplatesController(ISurveyTemplateService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<SurveyHeaderListDto>>> List(CancellationToken ct)
            => Ok(await _service.ListAsync(ct));

        [HttpGet("{headerId:int}")]
        public async Task<ActionResult<SurveyHeaderDetailDto>> Detail(int headerId, CancellationToken ct)
            => Ok(await _service.GetDetailAsync(headerId, ct));

        [HttpPost]
        public async Task<ActionResult<SurveyHeaderDetailDto>> Create([FromBody] SurveyHeaderCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateHeaderAsync(dto, ct);
            return CreatedAtAction(nameof(Detail), new { headerId = created.Id }, created);
        }

        [HttpPut("{headerId:int}")]
        public async Task<IActionResult> UpdateHeader(int headerId, [FromBody] SurveyHeaderUpdateDto dto, CancellationToken ct)
        {
            await _service.UpdateHeaderAsync(headerId, dto, ct);
            return NoContent();
        }

        [HttpDelete("{headerId:int}")]
        public async Task<IActionResult> DeleteHeader(int headerId, CancellationToken ct)
        {
            await _service.DeleteHeaderAsync(headerId, ct);
            return NoContent();
        }

        [HttpPost("{headerId:int}/items")]
        public async Task<ActionResult<SurveyItemDto>> AddItem(int headerId, [FromBody] SurveyItemCreateDto dto, CancellationToken ct)
            => Ok(await _service.AddItemAsync(headerId, dto, ct));

        [HttpPut("items/{itemId:int}")]
        public async Task<IActionResult> UpdateItem(int itemId, [FromBody] SurveyItemUpdateDto dto, CancellationToken ct)
        {
            await _service.UpdateItemAsync(itemId, dto, ct);
            return NoContent();
        }

        [HttpDelete("items/{itemId:int}")]
        public async Task<IActionResult> DeleteItem(int itemId, CancellationToken ct)
        {
            await _service.DeleteItemAsync(itemId, ct);
            return NoContent();
        }

        [HttpPost("{headerId:int}/items/reorder")]
        public async Task<IActionResult> Reorder(int headerId, [FromBody] ReorderItemDto dto, CancellationToken ct)
        {
            await _service.ReorderItemAsync(headerId, dto, ct);
            return NoContent();
        }
    }

}