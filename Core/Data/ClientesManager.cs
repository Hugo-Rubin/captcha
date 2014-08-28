using System.Linq;

namespace Core.Data
{
    public static class ClientesManager
    {
        private static readonly ocrdbEntities Db = new ocrdbEntities();

        public static Clientes GetCliente(string token)
        {
            return (from c in Db.Clientes
                    where c.Token == token
                    select c).FirstOrDefault();
        }

        public static Clientes GetCliente(int idCliente)
        {
            return (from c in Db.Clientes
                    where c.id == idCliente
                    select c).FirstOrDefault();
        } 
    }
}
