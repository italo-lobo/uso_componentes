using mvc_project.Models.Common;

namespace mvc_project.Models.Usuario
{
    public class UsuarioModel
    {
        public long id { get; set; }

        public string nombreUsuario { get; set; }

        public string nombrePersona { get; set; }

        public string apellidoPersona { get; set; }

        public string password { get; set; }
    }
}
