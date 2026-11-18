using System;

namespace ClasesBase
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }

        // Solo se usa al dar de alta o modificar: contraseña en texto plano.
        // Nunca se lee de la base de datos (alli se guarda el hash).
        public string Password { get; set; }
    }

    // Resultado posible de intentar iniciar sesion
    public enum ResultadoLogin
    {
        Correcto,
        UsuarioInexistente,
        PasswordIncorrecta,
        UsuarioInactivo
    }
}
