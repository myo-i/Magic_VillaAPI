using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Dto;
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IMapper _mapper;


        public VillaAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
        // awaitを使用する場合、メソッドにasyncを付与して返り値をTaskにしなければいけない
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            // Villasはテーブル
            //非同期で処理しているタスクの完了を待つ場合はawaitで明示する。
            IEnumerable<Villa> villasList = await _db.Villas.ToListAsync();

            // マッピングの変換先と変換元定義（これで他メソッドでもマッピングを利用できるようになる）
            // 返すのは単体のVillaDtoではなく、VillaDtoのリスト
            return Ok(_mapper.Map<List<VillaDto>>(villasList));
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
        // awaitを使用する場合、メソッドにasyncを付与して返り値をTaskにしなければいけない

        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            //非同期で処理しているタスクの完了を待つ場合はawaitで明示する。
            var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDto>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VillaDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto createDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            // Nameが重複した場合、エラー処理に入る
            if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
            {
                // 最初のパラメーターはキー名を表す
                ModelState.AddModelError("", "Villa already Exists!!");
                return BadRequest(ModelState);
            }
            if (createDto == null)
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

            // マッピングの変換先と変換元を定義
            Villa model = _mapper.Map<Villa>(createDto);

            await _db.Villas.AddAsync(model);
            // 変更をプッシュ
            await _db.SaveChangesAsync();

            // この関数内ではIdを持つのはmodelなのでvillaDto.Idからmodel.Idに変更
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            // 変更をプッシュ
            await _db.SaveChangesAsync();

            // 削除時は基本的にNoContentを返す
            return NoContent();
        }

        [HttpPut("id:int", Name="UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }
            // Idから自動的にどのレコードを更新するのか判断してくれるので削除
            //var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDto.Name;
            //villa.Sqft= villaDto.Sqft;
            //villa.Occupancy = villaDto.Occupancy;

            // マッピングの定義
            Villa model = _mapper.Map<Villa>(updateDto);

            _db.Villas.Update(model);
            // 変更をプッシュ
            await _db.SaveChangesAsync();


            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
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
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            // マッピングの定義
            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            // patchの適用
            patchDto.ApplyTo(villaDto, ModelState);


            // マッピングの定義
            Villa model = _mapper.Map<Villa>(villaDto);

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();


            // Modelが利用可能か判定
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }
}