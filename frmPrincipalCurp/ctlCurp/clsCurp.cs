using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ctlCurp {

    // La idea de hacer el generador de curp como clase a instanciar, fue para que solamente se pueda hacer
    // obligatoriamente 1 vez. También en caso de que se requiera usar o crear curps fuera del componente
    // en el proyecto que agregue esta biblioteca.

    public class clsCurp {
        // el FINAL de Java, en c#. Y el Map de c++, en c#
        // Diccionario para el calculo de la homo clave y el digito verificador.
        private readonly Dictionary<char, int> HOMOCLAVE = new Dictionary<char, int>{
            {'0', 0}, {'1', 1}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6},
            {'7', 7}, {'8', 8}, {'9', 9}, {'A', 10}, {'B', 11}, {'C', 12}, {'D', 13},
            {'E', 14}, {'F', 15}, {'G', 16}, {'H', 17}, {'I', 18}, {'J', 19}, {'K', 20},
            {'L', 21}, {'M', 22}, {'N', 23}, {'O', 24}, {'P', 25}, {'Q', 26}, {'R', 27},
            {'S', 28}, {'T', 29}, {'U', 30}, {'V', 31}, {'W', 32}, {'X', 33}, {'Y', 34},
            {'Z', 35}, {' ', 36}
        };

        // Tabla para derivar los caracteres de la homoclave.
        // El rango 0-23, es para evitar la ambiguedad segun la RENAPO (cociente)
        // mientras el rango 0-35 cubre toda la tabla (residuo)
        private readonly char[] TABLA_HOMOCLAVE = {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B',
            'C' ,'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        // Dicccionario del codigo de todos los estados.
        private readonly Dictionary<string, string> CODIGOS_ESTADOS = new Dictionary<string, string> {
            {"AGUASCALIENTES", "AS"}, {"BAJA CALIFORNIA", "BC"}, {"BAJA CALIFORNIA SUR", "BS"},
            {"CAMPECHE", "CC"}, {"CHIAPAS", "CS"}, {"CHIHUAHUA", "CH"}, {"COAHUILA", "CL"}, 
            {"COLIMA", "CM"}, {"DURANGO", "DG"}, {"GUANAJUATO", "GT"}, {"GUERRERO", "GR"}, 
            {"HIDALGO", "HG"}, {"JALISCO", "JC"}, {"MEXICO", "MN"}, {"CIUDAD DE MEXICO", "MC"}, // Actualizado de DF a MC
            {"MICHOACAN", "MS"}, {"NAYARIT", "NT"}, {"NUEVO LEON", "NL"}, {"OAXACA", "OC"}, 
            {"PUEBLA", "PL"}, {"QUERETARO", "QT"}, {"QUINTANA ROO", "QR"}, 
            {"SAN LUIS POTOSI", "SP"}, {"SINALOA", "SL"}, {"SONORA", "SR"}, {"TABASCO", "TC"},
            {"TAMAULIPAS", "TS"}, {"TLAXCALA", "TL"}, {"VERACRUZ", "VZ"}, {"YUCATAN", "YN"},
            {"ZACATECAS", "ZS"}, {"NACIDO EN EL EXTRANJERO", "NE"}
        };

        private string curp;

        // Variables ingresadas por el usuario.
        private string nombre;
        private string primerApellido;
        private string ultimoApellido;

        // La generación de la curp será directamente instanciando el objeto
        // más la obtención del string "curp" será unicamente por otro metodo.
        // si algun dato fue invalido, la curp se mantendrá como null.
        // Unicamente se podrá generar otra curp instanciando de nueva forma
        // un nuevo objeto de este tipo CON datos validos.
        public clsCurp(
            string nombre, string primerApellido, string ultimoApellido,
            DateTime fechaNacimiento, char genero, string estado
            ) {
            // Se creara una variable la cual verificara 4 datos.
            // Si uno delos 4 falla, entonces no hay forma de comprobar
            // o validar que se pueda generar la curp correctamente
            bool bDatosValidos = true;

            bDatosValidos = setNombres(nombre, out this.nombre) && this.nombre.Length >= 3;
            bDatosValidos = bDatosValidos && setNombres(primerApellido, out this.primerApellido);
            bDatosValidos = bDatosValidos && setNombres(ultimoApellido, out this.ultimoApellido);
            bDatosValidos = bDatosValidos && (DateTime.Now.CompareTo(fechaNacimiento) >= 0);

            curp = "";
            // Si los 3 datos escritos por el usuario son validos, podremos continuar con la
            // generacion de la curp;
            if (bDatosValidos) {
                curp += this.primerApellido[0];
                curp += getPrimerVocal(this.primerApellido);
                curp += this.ultimoApellido[0];
                curp += this.nombre[0];

                // Hay una regla que dice que si los 4 caracteres existe la posibilidad de que haya
                // CH o LL, cambiarle a X si es que estan en el inicio unicamente
                if (curp.IndexOf("CH", 0) == 0 || curp.IndexOf("LL", 0) == 0) {
                    curp = "X" + curp.Substring(2);
                }

                // Formato AA_MM_DD
                string anio = "" + (fechaNacimiento.Year % 100);
                anio = (anio.Length == 1 ? "0" + anio : anio);

                string mes = "" + (fechaNacimiento.Month);
                mes = (mes.Length == 1 ? "0" + mes : mes);

                string dia = "" + (fechaNacimiento.Day);
                dia = (dia.Length == 1 ? "0" + dia : dia);

                curp += anio + mes + dia;

                // Agregamos el genero H/M
                curp += genero;

                // Tomaremos el codigo de la entidad federativa
                curp += CODIGOS_ESTADOS[estado];

                // Ahora tomaremos las primeras consonantes internas
                // de apellido p, apellido m, nombre.
                curp += getPrimerConsonanteInterna(this.primerApellido);
                curp += getPrimerConsonanteInterna(this.ultimoApellido);
                curp += getPrimerConsonanteInterna(this.nombre);

                // Ahora se construira la homoclave
                string cadenaHomoc = this.primerApellido + " " + this.ultimoApellido + " " + this.nombre;
                long sumaHomoc = 0;

                for (int i = 0; i < cadenaHomoc.Length; i++) { 
                    char c = cadenaHomoc[i];
                    if (HOMOCLAVE.ContainsKey(c)) {
                        sumaHomoc += HOMOCLAVE[c] * (i + 1);
                    }
                }

                int valorHomoc = (int)(sumaHomoc % 1000);
                int cociente = valorHomoc / 34;
                int residuo = valorHomoc % 34;

                // Si el cociente esta dentro del rango 0-23.
                // Si no entra en la ambiguedad segun la RENAPO, entonces se usa 'Z'
                // como fallback general.
                if (cociente >= 0 && cociente <= 23) {
                    curp += TABLA_HOMOCLAVE[cociente];
                }
                else {
                    curp += 'Z';
                }
                

                // Directamente agregamos el residuo ya que este valor no supera
                // el rango de 34
                curp += TABLA_HOMOCLAVE[residuo];
            }
        }

        public string getCURP() {
            return curp;
        }

        // Este metodo será unicamente interno de la clase
        // Se hizo para validar si el nombre/apellido es valido y poder establecerselo
        // a la variable de referencia, si retorna falso, es por que el argumento
        // de nombre fue invalido por su formato.
        private bool setNombres(string nombre, out string variable) {
            bool bValido = false;

            // Si se pudo normalizar, podremos hacer la siguiente revision.
            if ((bValido = normalizarNombre(ref nombre)))
            {

                // comprobamos si la cadena normalizada es valida
                bValido = Regex.IsMatch(nombre, "^[A-Z\\s]+$");

            }

            // Si ninguna de las dos comprobaciones es correcta, se dejara la cadena vacia.
            if (!bValido) {
                nombre = string.Empty;
            }

            variable = nombre;
            return bValido;
        }

        private bool isVocal(char vocal) {
            return (vocal == 'A' || vocal == 'E' || vocal == 'I' || vocal == 'O' || vocal == 'U');
        }

        private bool isConsonante(char consonante) {
            return (consonante >= 'A' && consonante <= 'Z' && !isVocal(consonante));
        }

        // Pueden haber personas sin apellidos, por eso por default se regresa X.
        private char getPrimerVocal(string nombre) {
            foreach (char c in nombre) { 
                if (isVocal(c)) {
                    return c;
                }
            }
            return 'X';
        }

        // Para la generación de la curp se requiere de una consonante interna.
        // Pero para obtener una consonante interna se requiere minimo más de 2 caracteres.
        private char getPrimerConsonanteInterna(string nombre) { 
            if (nombre.Length < 2) {
                return 'X';
            }

            // Empezamos desde el segundo caracter e iremos buscando
            for (int i = 1; i < nombre.Length; i++) { 
                if (isConsonante(nombre[i])) {
                    return nombre[i];
                }
            }

            // Si no hay consonante, devuelve X.
            return 'X';
        }

        // Metodos utilizados para las validaciones

        // Metodo para normalizar (quitar acentos, reemplazar caracteres indicados
        // por la RENAPO, y tambien quitar prefijos). Retornara falso unicamente si no hay NADA que normalizar.
        // Puede ser un dato erroneo, y aún así retornara verdadero.
        private bool normalizarNombre(ref string nombre) {
            // Comprobamos si no esta vacia la cadena
            if (string.IsNullOrEmpty(nombre)) {
                nombre = string.Empty;
                return false;
            }

            // transformamos la cadena en mayusculas.
            nombre = nombre.ToUpperInvariant();

            // Se eliminara los acentos.
            // creamos una cadena normalizada para poder hacer la verificacion
            var nombreNormalizado = nombre.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            // iremos caracter por caracter, buscando los acentos y transformandolos en letras normales.
            foreach (char c in nombreNormalizado)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // construimos el string formado, y lo normalizamos a texto
            nombre = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // cambiaremos las Ñ, CH y LL
            nombre = nombre.Replace("Ñ", "X");
            nombre = nombre.Replace("CH", "C");
            nombre = nombre.Replace("LL", "L");

            // La expresión regular [^A-Z] busca cualquier caracter que NO sea una letra mayúscula (A-Z).
            // Y las elimina.
            nombre = Regex.Replace(nombre, @"[^A-Z\s]", "");

            // Remueve espacios múltiples para una limpieza más estricta.
            nombre = Regex.Replace(nombre, @"\s+", " ").Trim();

            // Lista de prefijos a ignorar, ordenados por longitud descendente para manejar
            // correctamente casos como "DE LA" antes que "DE".
            List<string> prefijos = new List<string> {
                "DE LAS ", "DE LOS ", "DE LA ", "DE EL ", "DEL ", "LAS ", 
                "LOS ", "LA ", "EL ", "Y ", "MAC ", "MC ", "VON ", "VAN ",
                "DE "
            };

            // Ordenamos por longitud descendente
            prefijos = prefijos.OrderByDescending(p => p.Length).ToList();

            // Aplicamos remoción de prefijos de forma iterativa
            // Se usa una aproximación iterativa para evitar desbordamiento de pila en cadenas muy largas
            // o con muchos prefijos anidados (aunque es poco probable en nombres/apellidos).
            string temp = nombre;
            bool bEliminando = true; // Flag para continuar mientras se sigan eliminando prefijos

            while (bEliminando)
            {
                bEliminando = false;
                foreach (var prefijo in prefijos)
                {
                    if (temp.StartsWith(prefijo))
                    {
                        temp = temp.Substring(prefijo.Length);
                        bEliminando = true; // Se removió un prefijo, intentar de nuevo con la lista completa
                        break; // Salir del foreach y reevaluar con la cadena modificada
                    }
                }
            }

            // Limpiar cualquier espacio extra que pueda haber quedado al inicio o final
            nombre = temp.Trim();

            // Si el primer nombre es MARIA, o JOSE, pero hay más nombres.
            // se tomara el siguiente nombre
            if (nombre.IndexOf("MARIA", 0) == 0 && nombre.Length > 6) {
                nombre = nombre.Substring(6);
            }
            else if (nombre.IndexOf("JOSE", 0) == 0 && nombre.Length > 5) {
                nombre = nombre.Substring(5);
            }

            return true;
        }

          

    }
}
