using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc_project.Models;
using mvc_project.Models.Common;
using mvc_project.Models.Login;
using mvc_project.Models.Usuario;
using service_library;

namespace mvc_project.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>(
                                "UsuarioLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            return View();
        }

        public IActionResult Nuevo()
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            UsuarioViewModel usuarioViewModel = new UsuarioViewModel
            {
                apellidoPersona = "",
                id = 0,
                nombrePersona = "",
                nombreUsuario = "",
                accion = CodigosAccion.Nuevo
            };

            return View("~/Views/Usuario/Usuario.cshtml", usuarioViewModel);
        }

        public IActionResult Editar(long idUsuario)
        {
            transversal_library.IUserService userService = new UserService();
            userService.GetUser("", "");

            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaUsuarios");

            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            UsuarioModel usuarioModel = list.Find(x => x.id == idUsuario);
            
            UsuarioViewModel usuarioViewModel = new UsuarioViewModel 
            {
                accion = CodigosAccion.Editar,
                apellidoPersona = usuarioModel.apellidoPersona,
                id = usuarioModel.id,
                nombrePersona = usuarioModel.nombrePersona,
                nombreUsuario = usuarioModel.nombreUsuario
            };

            return View("~/Views/Usuario/Usuario.cshtml", usuarioViewModel);
        }

        public IActionResult Ver(long idUsuario)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaUsuarios");

            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            UsuarioModel usuarioModel = list.Find(x => x.id == idUsuario);

            UsuarioViewModel usuarioViewModel = new UsuarioViewModel 
            {
                accion = CodigosAccion.Ver,
                apellidoPersona = usuarioModel.apellidoPersona,
                id = usuarioModel.id,
                nombrePersona = usuarioModel.nombrePersona,
                nombreUsuario = usuarioModel.nombreUsuario
            };

            return View("~/Views/Usuario/Usuario.cshtml", usuarioViewModel);
        }

        [HttpPost]
        public JsonResult Listar(QueryGridModel queryGridModel)
        {
            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaUsuarios");
            
            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            IEnumerable<UsuarioModel> listaUsuarios = list;
            if(queryGridModel.order != null && queryGridModel.order.Count > 0)
            {
                if(queryGridModel.columns[queryGridModel.order[0].column].name == "nombrePersona")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaUsuarios = list.OrderBy(x => x.nombrePersona);
                    }
                    else
                    {
                        listaUsuarios = list.OrderByDescending(x => x.nombrePersona);
                    }
                }
                else if(queryGridModel.columns[queryGridModel.order[0].column].name == "apellidoPersona")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaUsuarios = list.OrderBy(x => x.apellidoPersona);
                    }
                    else
                    {
                        listaUsuarios = list.OrderByDescending(x => x.apellidoPersona);
                    }
                }
                else if(queryGridModel.columns[queryGridModel.order[0].column].name == "nombreUsuario")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaUsuarios = list.OrderBy(x => x.nombreUsuario);
                    }
                    else
                    {
                        listaUsuarios = list.OrderByDescending(x => x.nombreUsuario);
                    }
                }
            }

            if(queryGridModel.search != null && queryGridModel.search.value != null)
            {
                listaUsuarios = listaUsuarios.Where(x => x.nombreUsuario.Contains(queryGridModel.search.value));
            }

            return Json(JsonReturn.SuccessWithInnerObject(new
            {
                draw = queryGridModel.draw,
                recordsFiltered = listaUsuarios.Count(),
                recordsTotal = list.Count,
                data = listaUsuarios
            }));
        }

        [HttpPost]
        public JsonResult Guardar(UsuarioModel usuarioModel)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Json(Models.Common.JsonReturn.Redirect("Home/Index"));
            }

            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaUsuarios");

            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            if(usuarioModel.id == 0)
            {
                usuarioModel.id = list.Count + 1;
                list.Add(usuarioModel);
            }
            else
            {
                UsuarioModel usuario = list.Find(x => x.id == usuarioModel.id);
                usuario.apellidoPersona = usuarioModel.apellidoPersona;
                usuario.nombrePersona = usuarioModel.nombrePersona;
                if(!string.IsNullOrEmpty(usuarioModel.password))
                {
                    usuario.password = usuarioModel.password;
                }
                usuario.nombreUsuario = usuarioModel.nombreUsuario;
            }

            HttpContext.Session.Set<List<UsuarioModel>>("ListaUsuarios", list);

            return Json(JsonReturn.SuccessWithoutInnerObject());
        }

        [HttpPost]
        public JsonResult Eliminar(long id)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Json(Models.Common.JsonReturn.Redirect("Home/Index"));
            }

            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaUsuarios");

            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            UsuarioModel usuario = list.Find(x => x.id == id);
            
            if(usuario == null)
            {
                return Json(Models.Common.JsonReturn.ErrorWithSimpleMessage("El usuario que desea eliminar no existe más"));
            }
            
            list.Remove(usuario);
            
            HttpContext.Session.Set<List<UsuarioModel>>("ListaUsuarios", list);

            return Json(JsonReturn.SuccessWithoutInnerObject());
        }
    }
}
