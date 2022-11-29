using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Dto;
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VillaApi")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }

        [HttpGet("{id:int}", Name ="GetVilla")]
        // APIの規約を作成
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // 上記のほうがより説明的
        //[ProducesResponseType(200, Type = typeof(VillaDto))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]

        public ActionResult<VillaDto> GetVilla(int id)
        {

            if (id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VillaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CreateVilla([FromBody] VillaDto villaDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            // Nameが重複した場合、エラー処理に入る
            if(VillaStore.villaList.FirstOrDefault(u=>u.Name.ToLower()==villaDto.Name.ToLower()) != null)
            {
                // 最初のパラメーターはキー名を表す
                ModelState.AddModelError("", "Villa already Exists!!");
                return BadRequest(ModelState);
            }
            if (villaDto == null)
            {
                return BadRequest();
            }

            // Idはデフォルト以外の値を入力するとエラー
            if (villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // Id降順（4,3,2,1のような）で並べた際の一番最初のId+1を代入
            villaDto.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDto);

            return CreatedAtRoute("GetVilla", new { id = villaDto.Id}, villaDto);
        }

        [HttpDelete("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u=>u.Id== id);
            if(villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);

            // 削除時は基本的にNoContentを返す
            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDto villaDto)
        {
            if(villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDto.Name;
            villa.Sqft= villaDto.Sqft;
            villa.Occupancy = villaDto.Occupancy;

            return NoContent();

        }
    }
}