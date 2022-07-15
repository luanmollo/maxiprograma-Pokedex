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
                comando.CommandText = "select p.Id, Numero, Nombre, p.Descripcion, UrlImagen, e.Descripcion as Tipo, d.Descripcion as Debilidad, p.IdTipo, p.IdDebilidad from pokemons p, elementos e, elementos d where p.IdTipo = e.Id and p.IdDebilidad = d.Id and Activo = 1";
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

        public void Ocultar(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.ConfigurarConsulta("update pokemons set activo = 0 where id = @Id");
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

        public List<Pokemon> Filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "select p.Id, Numero, Nombre, p.Descripcion, UrlImagen, e.Descripcion as Tipo, d.Descripcion as Debilidad, p.IdTipo, p.IdDebilidad from pokemons p, elementos e, elementos d where p.IdTipo = e.Id and p.IdDebilidad = d.Id and Activo = 1 ";

                switch (campo)
                {
                    case "Número":
                        switch (criterio)
                        {
                            case "Mayor a":
                                consulta += " and Numero > " + filtro;
                                break;
                            case "Menor a":
                                consulta += " and Numero < " + filtro;
                                break;
                            case "Igual a":
                                consulta += " and Numero = " + filtro;
                                break;
                        }
                        break;

                    case "Nombre":
                        switch (criterio)
                        {
                            case "Comienza con":
                                consulta += " and Nombre like '" + filtro + "%'";
                                break;
                            case "Termina con":
                                consulta += " and Nombre like '%" + filtro + "'";
                                break;
                            case "Contiene":
                                consulta += " and Nombre like '%" + filtro + "%'";
                                break;
                        }
                       break;

                    case "Descripción":
                        switch (criterio)
                        {
                            case "Comienza con":
                                consulta += " and p.Descripcion like '" + filtro + "%'";
                                break;
                            case "Termina con":
                                consulta += " and p.Descripcion like '%" + filtro + "'";
                                break;
                            case "Contiene":
                                consulta += " and p.Descripcion like '%" + filtro + "%'";
                                break;
                        }
                        break;
                }

                datos.ConfigurarConsulta(consulta);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();

                    aux.Id = (int)datos.Lector["Id"];
                    aux.Numero = (int)datos.Lector["Numero"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];


                    if (!(datos.Lector["UrlImagen"] is DBNull))
                        aux.UrlImagen = (string)datos.Lector["UrlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];

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
                datos.CerrarConexion();
            }
        }

    }
}
