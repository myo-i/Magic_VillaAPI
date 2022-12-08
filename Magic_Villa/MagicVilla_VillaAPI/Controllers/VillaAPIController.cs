using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Dto;
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VillaApi")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        // private変数は"_"でハイライトしたりする
        //private readonly ILogger<VillaAPIController> _logger;

        private readonly ApplicationDbContext _db;


        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            // Villasはテーブル
            return Ok(_db.Villas);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
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

            var villa = _db.Villas.FirstOrDefault(x => x.Id == id);

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
        public ActionResult<VillaDto> CreateVilla([FromBody] VillaCreateDto villaDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            // Nameが重複した場合、エラー処理に入る
            if (_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
            {
                // 最初のパラメーターはキー名を表す
                ModelState.AddModelError("", "Villa already Exists!!");
                return BadRequest(ModelState);
            }
            if (villaDto == null)
            {
                return BadRequest();
            }

            // CreateではIdは自動生成なので必要ない
            // Idはデフォルト以外の値を入力するとエラー
            //if (villaDto.Id > 0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            // Id降順（4,3,2,1のような）で並べた際の一番最初のId+1を代入
            // Idは自動的に生成されるため削除
            //villaDto.Id = _db.Villas.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;

            // VillaDtoを暗黙的にVillaに変換できないため、VillaDtoの値をVillaに渡している
            Villa model = new Villa()
            {
                Name = villaDto.Name,
                Rate = villaDto.Rate,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                Occupancy = villaDto.Occupancy,
                Sqft = villaDto.Sqft,
                Amenity = villaDto.Amenity,

            };
            _db.Villas.Add(model);
            // 変更をプッシュ
            _db.SaveChanges();

            // この関数内ではIdを持つのはmodelなのでvillaDto.Idからmodel.Idに変更
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
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
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            // 変更をプッシュ
            _db.SaveChanges();

            // 削除時は基本的にNoContentを返す
            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
        {
            if (villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            // Idから自動的にどのレコードを更新するのか判断してくれるので削除
            //var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDto.Name;
            //villa.Sqft= villaDto.Sqft;
            //villa.Occupancy = villaDto.Occupancy;
            Villa model = new Villa()
            {
                Id = villaDto.Id,
                Name = villaDto.Name,
                Rate = villaDto.Rate,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                Occupancy = villaDto.Occupancy,
                Sqft = villaDto.Sqft,
                Amenity = villaDto.Amenity,
            };
            _db.Villas.Update(model);
            // 変更をプッシュ
            _db.SaveChanges();


            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            //if (villa == null)
            //{
            //    return BadRequest();
            //}

            // DBで同時に2つのIdを追跡することはできないため、追跡しないように設定する
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);

            // patchのみ完全なオブジェクトを取得するのではなく、更新が必要なフィールドのみを受け取っているので
            // ここではVillasをVillaDtoに変換する必要がある
            VillaUpdateDto villaDto = new VillaUpdateDto()
            {
                Id = villa.Id,
                Name = villa.Name,
                Rate = villa.Rate,
                Details = villa.Details,
                ImageUrl = villa.ImageUrl,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft,
                Amenity = villa.Amenity,
            };

            // patchの適用
            patchDto.ApplyTo(villaDto, ModelState);

            // Modelが利用可能か判定
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // EntityFramework Coreで更新を呼び出すにはVillaである必要があるため
            // VillaDtoをVillaに戻す必要がある
            Villa model = new Villa()
            {
                Id = villaDto.Id,
                Name = villaDto.Name,
                Rate = villaDto.Rate,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                Occupancy = villaDto.Occupancy,
                Sqft = villaDto.Sqft,
                Amenity = villaDto.Amenity,
            };
            _db.Villas.Update(model);
            _db.SaveChanges();


            return NoContent();
        }

    }
}