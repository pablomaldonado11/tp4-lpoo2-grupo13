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
        public string Password { get; set; }
    }

    public enum ResultadoLogin
    {
        Correcto,
        UsuarioInexistente,
        PasswordIncorrecta,
        UsuarioInactivo
    }
}
