﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SIGEPROAVI_API.Models;
using SIGEPROAVI_API.DTO;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;

namespace SIGEPROAVI_API.Controllers
{
    public class Seg_UsuarioController : ApiController
    {
        private SIGEPROAVI_APIContext db = new SIGEPROAVI_APIContext();

        // GET: api/Seg_Usuario
        //public IQueryable<Seg_Usuario> GetSeg_Usuario()
        //{
        //    return db.Seg_Usuario;
        //}
        public IQueryable<Seg_Usuario_ConsultaDTO> GetSeg_Usuario()
        {
            var consulta = from U in db.Seg_Usuario
                           from TU in db.Seg_Tipo_Usuario.Where(TU => TU.IdSegTipoUsuario == U.IdSegTipoUsuario)
                           select new Seg_Usuario_ConsultaDTO
                           {
                               ApellidoMaterno = U.ApellidoMaterno,
                               IdSegTipoUsuario = U.IdSegTipoUsuario,
                               ApellidoPaterno = U.ApellidoPaterno,
                               Clave = U.Clave,
                               DescripcionTipoUsuario = TU.Descripcion,
                               IdSegUsuario = U.IdSegUsuario,
                               Nombres = U.Nombres,
                               Usuario = U.Usuario
                           };

            return consulta;
        }

        // GET: api/Seg_Usuario/5
        //[ResponseType(typeof(Seg_Usuario))]
        //public async Task<IHttpActionResult> GetSeg_Usuario(int id)
        //{
        //    Seg_Usuario seg_Usuario = await db.Seg_Usuario.FindAsync(id);
        //    if (seg_Usuario == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(seg_Usuario);
        //}
        public IQueryable<Seg_Usuario_ConsultaDTO> GetSeg_Usuario(int id)
        {
            var consulta = from U in db.Seg_Usuario.Where(U => U.IdSegUsuario == id)
                           from TU in db.Seg_Tipo_Usuario.Where(TU => TU.IdSegTipoUsuario == U.IdSegTipoUsuario)
                           select new Seg_Usuario_ConsultaDTO
                           {
                               ApellidoMaterno = U.ApellidoMaterno,
                               IdSegTipoUsuario = U.IdSegTipoUsuario,
                               ApellidoPaterno = U.ApellidoPaterno,
                               Clave = U.Clave,
                               DescripcionTipoUsuario = TU.Descripcion,
                               IdSegUsuario = U.IdSegUsuario,
                               Nombres = U.Nombres,
                               Usuario = U.Usuario
                           };

            return consulta;
        }

        // PUT: api/Seg_Usuario/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSeg_Usuario(int id, Seg_Usuario_ModificacionDTO seg_UsuarioM)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != seg_UsuarioM.IdSegUsuario)
            {
                return BadRequest();
            }

            Seg_Usuario seg_Usuario = await db.Seg_Usuario.FindAsync(id);
            seg_Usuario.Estado = seg_UsuarioM.Estado;
            seg_Usuario.FechaModificacion = DateTime.Now;
            seg_Usuario.Nombres = seg_UsuarioM.Nombres;
            seg_Usuario.ApellidoMaterno = seg_UsuarioM.ApellidoMaterno;
            seg_Usuario.ApellidoPaterno = seg_UsuarioM.ApellidoPaterno;
            seg_Usuario.IdSegTipoUsuario = seg_UsuarioM.IdSegTipoUsuario;
            seg_Usuario.UsuarioModificador = seg_UsuarioM.UsuarioModificador;

            if (seg_Usuario.Clave != seg_UsuarioM.Clave)
            {
                seg_Usuario.Clave = MD5Hash(seg_UsuarioM.Clave);
            }

            db.Entry(seg_Usuario).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Seg_UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return StatusCode(HttpStatusCode.NoContent);

            //Seg_Usuario_ConsultaDTO resultado = GetSeg_Usuario();

            //return Ok(db.Seg_Usuario);
            return Ok(GetSeg_Usuario());
        }

        // POST: api/Seg_Usuario
        [ResponseType(typeof(Seg_Usuario))]
        //public async Task<IHttpActionResult> PostSeg_Usuario(Seg_Usuario seg_Usuario)
        //{
        //    var errors = ModelState.Values.SelectMany(v => v.Errors);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Seg_Usuario.Add(seg_Usuario);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = seg_Usuario.IdSegUsuario }, seg_Usuario);
        //}
        public async Task<IHttpActionResult> PostSeg_Usuario(Seg_Usuario_InsercionDTO seg_UsuarioI)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Seg_Usuario_InsercionDTO, Seg_Usuario>());

            Seg_Usuario seg_Usuario = Mapper.Map<Seg_Usuario>(seg_UsuarioI);
            seg_Usuario.FechaCreacion = DateTime.Now;
            seg_Usuario.Clave = MD5Hash(seg_UsuarioI.Clave);
            seg_Usuario.Estado = true;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Seg_Usuario.Add(seg_Usuario);
            await db.SaveChangesAsync();

            //return CreatedAtRoute("DefaultApi", new { id = seg_Usuario.IdSegUsuario }, GetSeg_Usuario(seg_Usuario.IdSegUsuario));
            return Ok(GetSeg_Usuario(seg_Usuario.IdSegUsuario));
        }

        // DELETE: api/Seg_Usuario/5
        [ResponseType(typeof(Seg_Usuario))]
        public async Task<IHttpActionResult> DeleteSeg_Usuario(int id)
        {
            Seg_Usuario seg_Usuario = await db.Seg_Usuario.FindAsync(id);
            if (seg_Usuario == null)
            {
                return NotFound();
            }

            db.Seg_Usuario.Remove(seg_Usuario);
            await db.SaveChangesAsync();

            return Ok(seg_Usuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Seg_UsuarioExists(int id)
        {
            return db.Seg_Usuario.Count(e => e.IdSegUsuario == id) > 0;
        }

        [HttpPost]
        [Route("api/Seg_Usuario/Login")]
        [ResponseType(typeof(Seg_Usuario))]
        public async Task<IHttpActionResult> Login(Seg_Usuario seg_UsuarioL)
        {
            string clave = MD5Hash(seg_UsuarioL.Clave);

            Seg_Usuario seg_Usuario = await db.Seg_Usuario.Where(u => u.Usuario == seg_UsuarioL.Usuario && u.Clave == clave).FirstOrDefaultAsync();
            if (seg_Usuario == null)
            {
                return NotFound();
            }

            return Ok(seg_Usuario);
        }

        public static string MD5Hash(string text)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();

                //compute hash from the bytes of text
                md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

                //get hash result after compute it
                byte[] result = md5.Hash;

                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    //change it into 2 hexadecimal digits
                    //for each byte
                    strBuilder.Append(result[i].ToString("x2"));
                }

                return strBuilder.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}