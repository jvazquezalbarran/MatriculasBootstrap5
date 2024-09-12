using MatriculacionesAlumnosAPP.Models;
using MatriculacionesAlumnosAPP.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatriculacionesAlumnosAPP.Controllers
{
    public class MatriculaController : Controller
    {
        // GET: Matricula
        public ActionResult CrearMatricula()
        {
            MatriculasAlumnosViewModel vm = new MatriculasAlumnosViewModel();
            using(var context = new DbContext.Context())
            {
                vm.Alumnos = context.Alumnos.OrderByDescending(a => a.nombreAlumno).ToList();
                vm.Asignaturas = context.Asignaturas.OrderByDescending(a => a.nombreAsignatura).ToList();
            }

            return View(vm);
        }
        [HttpPost]
        public RedirectToRouteResult CrearMatricula(MatriculasAlumnosViewModel vm)
        {
            using(var context = new DbContext.Context())
            {
                var alumno = context.Alumnos.Where(a => a.AlumnoID 
                        == vm.AlumnoSeleccionado).FirstOrDefault();
                var asignatura = context.Asignaturas.Where(a => a.AsignaturaID 
                        == vm.AsignaturaSeleccionada).FirstOrDefault();

                Matricula matricula = new Matricula();
                matricula.Alumno = alumno;
                matricula.AlumnoID = Convert.ToInt32(vm.AlumnoSeleccionado);
                matricula.Asignatura = asignatura;
                matricula.AsignaturaID = Convert.ToInt32(vm.AsignaturaSeleccionada);
                matricula.Fecha_Matricula = vm.FechaMatricula;

                context.Matriculas.Add(matricula);
                context.SaveChanges();
                    
            }
            return RedirectToAction("VerMatriculas");
        }

        public PartialViewResult _CreatePartial()
        {
            var model = new MatriculasAlumnosViewModel();
            using (var context = new DbContext.Context())
            {
                model.Alumnos = context.Alumnos.OrderByDescending(a => a.nombreAlumno).ToList();
                model.Asignaturas = context.Asignaturas.OrderByDescending(a => a.nombreAsignatura).ToList();
            }
            return PartialView(model);
        }

        public ActionResult VerMatriculas(bool id, string sortOrder, string currentFilter, string searchString, int? page)
        //parametros: id muestra u oculta la caja de busqueda
        //sortorder: permite hacer ordenación
        //searchString: se encarga de hacer la busqueda
        //currentFilter: recibe el valor del filtro actual
        //page: nº de página a mostrar
        {
            ViewBag.buscador = id;
            //Parámetro con la Ordenacion actual
            ViewBag.CurrentSort = sortOrder;
            //Parametros para la Ordenación de resultados
            ViewBag.NameSortrParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            //Control del numero de página desde la que se ha hecho la búsqueda
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            using (var context = new DbContext.Context())
            {
                var queryConIncludes = context.Matriculas.Include(m => m.Alumno).Include(m => m.Asignatura).ToList();
                if (!string.IsNullOrEmpty(searchString))
                {
                    queryConIncludes = context.Matriculas.Include(m => m.Alumno).
                        Include(m => m.Asignatura).
                        Where(m => m.Alumno.nombreAlumno.Contains(searchString) || m.Asignatura.nombreAsignatura.Contains(searchString)).ToList();
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        queryConIncludes = queryConIncludes.OrderByDescending(s => s.Alumno.nombreAlumno).ToList();
                        break;
                    case "Date":
                        queryConIncludes = queryConIncludes.OrderBy(s => s.Fecha_Matricula).ToList();
                        break;
                    case "date_desc":
                        queryConIncludes = queryConIncludes.OrderByDescending(s => s.Fecha_Matricula).ToList();
                        break;
                    default:
                        queryConIncludes = queryConIncludes.OrderBy(s => s.Alumno.nombreAlumno).ToList();
                        break;
                }

                int pageSize = 3; //numero de registros por pagina
                int pageNumber = (page ?? 1); //Devuelve la pagina si tiene un valor o devuelve 1 si es null

                return View("VerMatriculasConInclude", queryConIncludes.ToPagedList(pageNumber, pageSize));
            }
        }
        public RedirectToRouteResult EliminarMatricula(int id)
        {
            using(var context = new DbContext.Context())
            {
                var query = context.Matriculas.Where(m => m.MatriculaID == id).SingleOrDefault();
                
                if(query != null)
                {
                    context.Matriculas.Remove(query);
                    context.SaveChanges();
                    return RedirectToAction("VerMatriculas");
                }
                else
                {
                    return null;
                }    

            }       
        }

    }
}