﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Datos
{
    class SqlHelper
    {
        public SqlHelper()
        {

        }
        #region Parámetros de conexión

        /// <summary>
        /// Cambiar acá en caso de hacer referencia a su servidor sql local.
        /// </summary>
        private string strNombreCadenaConexion = "enmotooConnection";

        /// <summary>
        /// Nombre de la cadena de conexión agregada en el archivo de configuración en la sección connectionStrings
        /// </summary>
        public string NombreCadenaConexion
        {
            get
            {
                return strNombreCadenaConexion;
            }
            set
            {
                strNombreCadenaConexion = value;
                conexion.ConnectionString = ConfigurationManager.ConnectionStrings[NombreCadenaConexion].ConnectionString;
            }
        }
        /// <summary>
        /// Cambiar acá también en caso de hacer referencia a su servidor sql local.
        /// </summary>
        public SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["enmotooConnection"].ConnectionString);

        /// <summary>
        /// Establece los parámetro de la cadena de conexión a utilizar
        /// </summary>
        /// <param name="strLogin">Usuario SQL de conexión</param>
        /// <param name="strPassword">Contraseña SQL de conexión</param>
        public void ParametrosConexion(string strLogin, string strPassword)
        {
            SqlConnectionStringBuilder conexBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[NombreCadenaConexion].ConnectionString);
            conexBuilder.UserID = strLogin;
            conexBuilder.Password = strPassword;
            conexion.ConnectionString = conexBuilder.ConnectionString;
        }

        /// <summary>
        /// Establece los parámetro de la cadena de conexión a utilizar
        /// </summary>
        /// <param name="strLogin">Usuario SQL de conexión</param>
        /// <param name="strPassword">Contraseña SQL de conexión</param>
        /// <param name="strBaseDatos">Nombre de la base de datos a la que se conectará</param>
        /// <param name="strServidor">Nombre del servidor al que se conectará</param>
        public void ParametrosConexion(string strLogin, string strPassword, string strBaseDatos, string strServidor)
        {
            SqlConnectionStringBuilder conexBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[NombreCadenaConexion].ConnectionString);
            conexBuilder.UserID = strLogin;
            conexBuilder.Password = strPassword;
            conexBuilder.InitialCatalog = strBaseDatos;
            conexBuilder.DataSource = strServidor;
            conexion.ConnectionString = conexBuilder.ConnectionString;
        }

        public void CerrarConexion()
        {
            if (conexion.State != ConnectionState.Closed)
                conexion.Close();
        }

        #endregion

        #region Métodos de ejecución de consultas de selección

        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public DataSet EjecutarDataSet(CommandType tipoComando, string strSQL, bool ynCerrarConexion)
        {
            return EjecutarDataSet(tipoComando, strSQL, null, ynCerrarConexion);
        }

        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="parametros">Colección de parámetros a utilizar en la consulta</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public DataSet EjecutarDataSet(CommandType tipoComando, string strSQL, SqlParameter[] parametros, bool ynCerrarConexion)
        {
            using (SqlCommand comando = new SqlCommand())
            {
                using (DataSet dsDatos = new DataSet())
                {
                    comando.CommandType = tipoComando;
                    comando.CommandText = strSQL;
                    comando.CommandTimeout = 80000;
                    if (parametros != null && parametros.Length > 0)
                    {
                        comando.Parameters.AddRange(parametros);
                    }

                    comando.Connection = conexion;
                    if (conexion.State != ConnectionState.Open)
                        conexion.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(comando);
                    adapter.Fill(dsDatos);
                    if (ynCerrarConexion)
                        conexion.Close();
                    comando.Parameters.Clear();
                    return dsDatos;
                }
            }
        }
        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="parametros">Colección de parámetros a utilizar en la consulta</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <param name="transaction">Objeto transacción padre, si hay un problema, se anulan todas las transacciones asociadas.</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public DataSet EjecutarDataSetTransaction(CommandType tipoComando, string strSQL, SqlParameter[] parametros, bool ynCerrarConexion, SqlTransaction transaction)
        {
            using (SqlCommand comando = new SqlCommand())
            {
                using (DataSet dsDatos = new DataSet())
                {
                    comando.CommandType = tipoComando;
                    comando.CommandText = strSQL;
                    comando.CommandTimeout = 80000;
                    if (parametros != null && parametros.Length > 0)
                    {
                        comando.Parameters.AddRange(parametros);
                    }
                    comando.Connection = conexion;
                    comando.Transaction = transaction;
                    if (conexion.State != ConnectionState.Open)
                        conexion.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(comando);
                    adapter.Fill(dsDatos);
                    if (ynCerrarConexion)
                        conexion.Close();
                    comando.Parameters.Clear();
                    return dsDatos;
                }
            }
        }
        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public DataTable EjecutarDataTable(CommandType tipoComando, string strSQL, bool ynCerrarConexion)
        {
            return EjecutarDataSet(tipoComando, strSQL, null, ynCerrarConexion).Tables[0];
        }

        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="parametros">Colección de parámetros a utilizar en la consulta</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public DataTable EjecutarDataTable(CommandType tipoComando, string strSQL, SqlParameter[] parametros, bool ynCerrarConexion)
        {
            return EjecutarDataSet(tipoComando, strSQL, parametros, ynCerrarConexion).Tables[0];
        }
        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="parametros">Colección de parámetros a utilizar en la consulta</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <param name="transaction">Objeto transacción padre, si hay un problema, se anulan todas las transacciones asociadas.</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public DataTable EjecutarDataTableTransaction(CommandType tipoComando, string strSQL, SqlParameter[] parametros, bool ynCerrarConexion, SqlTransaction transaction)
        {
            return EjecutarDataSetTransaction(tipoComando, strSQL, parametros, ynCerrarConexion, transaction).Tables[0];
        }

        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public SqlDataReader EjecutarReader(CommandType tipoComando, string strSQL, bool ynCerrarConexion)
        {
            return EjecutarReader(tipoComando, strSQL, null, ynCerrarConexion);
        }

        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="parametros">Colección de parámetros a utilizar en la consulta</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public SqlDataReader EjecutarReader(CommandType tipoComando, string strSQL, SqlParameter[] parametros, bool ynCerrarConexion)
        {
            using (SqlCommand comando = new SqlCommand())
            {
                SqlDataReader reader;
                comando.CommandType = tipoComando;
                comando.CommandText = strSQL;
                if (parametros != null && parametros.Length > 0)
                {
                    comando.Parameters.AddRange(parametros);
                }
                comando.Connection = conexion;
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();
                reader = comando.ExecuteReader();
                if (ynCerrarConexion)
                    conexion.Close();
                comando.Parameters.Clear();
                return reader;
            }
        }

        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public XmlReader EjecutarXmlReader(CommandType tipoComando, string strSQL, bool ynCerrarConexion)
        {
            return EjecutarXmlReader(tipoComando, strSQL, null, ynCerrarConexion);
        }

        /// <summary>
        /// Ejecuta una consulta o procedimiento SQL que devuelve datos.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="parametros">Colección de parámetros a utilizar en la consulta</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Retorna el conjunto de datos de la instrucción SQL.</returns>
        public XmlReader EjecutarXmlReader(CommandType tipoComando, string strSQL, SqlParameter[] parametros, bool ynCerrarConexion)
        {
            using (SqlCommand comando = new SqlCommand())
            {
                XmlReader reader;
                comando.CommandType = tipoComando;
                comando.CommandText = strSQL;
                if (parametros != null && parametros.Length > 0)
                {
                    comando.Parameters.AddRange(parametros);
                }
                comando.Connection = conexion;
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();
                reader = comando.ExecuteXmlReader();
                if (ynCerrarConexion)
                    conexion.Close();
                comando.Parameters.Clear();
                return reader;
            }
        }

        #endregion

        #region Método de ejecución de consultas Insert, Update y Delete

        /// <summary>
        /// Ejecuta la consulta y devuelve la primera columna de la primera fila del conjunto de resultados devueltos por la consulta.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Devuelve la primera columna de la primera fila del conjunto de resultados devueltos por la consulta.</returns>
        public object EjecutarScalar(CommandType tipoComando, string strSQL, bool ynCerrarConexion)
        {
            return EjecutarScalar(tipoComando, strSQL, null, ynCerrarConexion);
        }

        /// <summary>
        /// Ejecuta la consulta y devuelve la primera columna de la primera fila del conjunto de resultados devueltos por la consulta.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="parametros">Colección de parámetros a utilizar en la consulta</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Devuelve la primera columna de la primera fila del conjunto de resultados devueltos por la consulta.</returns>
        public object EjecutarScalar(CommandType tipoComando, string strSQL, SqlParameter[] parametros, bool ynCerrarConexion)
        {
            using (SqlCommand comando = new SqlCommand())
            {
                object objRetorno = new object();
                comando.CommandType = tipoComando;
                comando.CommandText = strSQL;
                if (parametros != null && parametros.Length > 0)
                {
                    comando.Parameters.AddRange(parametros);
                }
                comando.Connection = conexion;
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();
                objRetorno = comando.ExecuteScalar();
                if (ynCerrarConexion)
                    conexion.Close();
                comando.Parameters.Clear();
                return objRetorno;
            }
        }

        /// <summary>
        /// Ejecuta una instrucción Transact-SQL y devuelve en número de filas afectadas.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Devuelve en número de filas afectadas.</returns>
        public int EjecutarNonQuery(CommandType tipoComando, string strSQL, bool ynCerrarConexion)
        {
            return EjecutarNonQuery(tipoComando, strSQL, null, ynCerrarConexion);
        }

        /// <summary>
        /// Ejecuta una instrucción Transact-SQL y devuelve en número de filas afectadas.
        /// </summary>
        /// <param name="tipoComando">Tipo de comando a ejecutar: Tabla, Texto, Procedimiento</param>
        /// <param name="strSQL">Consulta sql, nombre del procedimiento o nombre de tabla, según el tipo de comando a utilizar</param>
        /// <param name="parametros">Colección de parámetros a utilizar en la consulta</param>
        /// <param name="ynCerrarConexion">Indica si la conexión se cerrará después de ejecutar la transacción</param>
        /// <returns>Devuelve en número de filas afectadas.</returns>
        public int EjecutarNonQuery(CommandType tipoComando, string strSQL, SqlParameter[] parametros, bool ynCerrarConexion)
        {
            using (SqlCommand comando = new SqlCommand())
            {
                int intRetorno;
                comando.CommandType = tipoComando;
                comando.CommandText = strSQL;
                if (parametros != null && parametros.Length > 0)
                {
                    comando.Parameters.AddRange(parametros);
                }
                comando.Connection = conexion;
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();
                intRetorno = comando.ExecuteNonQuery();
                if (ynCerrarConexion)
                    conexion.Close();
                comando.Parameters.Clear();
                return intRetorno;
            }
        }

        #endregion
    }
}

