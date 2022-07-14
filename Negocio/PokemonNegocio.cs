using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;

namespace Negocio
{
    public class PokemonNegocio
    {
        public List<Pokemon> Listar()
        {
            List<Pokemon> lista = new List<Pokemon>();

            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;


            try
            {
                conexion.ConnectionString = "server=CYS160PC\\SQLEXPRESS; database=POKEDEX_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "select p.Id, Numero, Nombre, p.Descripcion, UrlImagen, e.Descripcion as Tipo, d.Descripcion as Debilidad, p.IdTipo, p.IdDebilidad from pokemons p, elementos e, elementos d where  p.IdTipo = e.Id and p.IdDebilidad = d.Id";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Pokemon aux = new Pokemon();

                    /*
                    aux.Numero = lector.GetInt32(0);
                    aux.Nombre = lector.GetString(1);
                    aux.Descripcion = lector.GetString(2);
                    */

                    aux.Id = (int)lector["Id"];
                    aux.Numero = (int)lector["Numero"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    /*
                    if (!(lector.IsDBNull(lector.GetOrdinal("UrlImagen"))))
                    {
                        aux.UrlImagen = (string)lector["UrlImagen"];
                    } 
                    */

                    if (!(lector["UrlImagen"] is DBNull))
                        aux.UrlImagen = (string)lector["UrlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];

                    lista.Add(aux);

                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conexion.Close();
            }

        }

        public void Agregar(Pokemon nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            
            try
            {
                datos.ConfigurarConsulta("insert into POKEMONS(Numero, Nombre, Descripcion, UrlImagen, IdTipo, IdDebilidad, Activo) values (" + nuevo.Numero + ", '" + nuevo.Nombre + "', '" + nuevo.Descripcion + "', @UrlImagen, @IdTipo , @IdDebilidad , 1)");
                datos.ConfigurarParametros("@UrlImagen", nuevo.UrlImagen);
                datos.ConfigurarParametros("@IdTipo", nuevo.Tipo.Id);
                datos.ConfigurarParametros("@IdDebilidad", nuevo.Debilidad.Id);
                datos.EjecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void Modificar(Pokemon modificar)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.ConfigurarConsulta("update pokemons set Numero = @Numero, Nombre = @Nombre, Descripcion = @Descripcion, UrlImagen = @UrlImagen, IdTipo = @IdTipo, IdDebilidad = @IdDebilidad where id=@Id");
                datos.ConfigurarParametros("@Numero", modificar.Numero);
                datos.ConfigurarParametros("@Nombre", modificar.Nombre);
                datos.ConfigurarParametros("@Descripcion", modificar.Descripcion);
                datos.ConfigurarParametros("@UrlImagen", modificar.UrlImagen);
                datos.ConfigurarParametros("@IdTipo", modificar.Tipo.Id);
                datos.ConfigurarParametros("@IdDebilidad", modificar.Debilidad.Id);
                datos.ConfigurarParametros("@Id", modificar.Id);

                datos.EjecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void Eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.ConfigurarConsulta("delete pokemons where id= @Id");
                datos.ConfigurarParametros("@Id", id);
                datos.EjecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

    }
}
