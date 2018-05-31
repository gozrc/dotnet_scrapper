using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Commons.CustomHttpManager.Hacks
{
    public class HackSucuri
    {
        public static bool checkSucuriProtection (string response)
        {
            return true;
        }

        public static bool addSucuriHeaders (string jsCrypt, ref HttpHeaders headers, ref string error)
        {
            try
            {
                Regex reg = new Regex("S='.+?'", RegexOptions.Singleline);

                string lin0 = string.Empty;
                string lin1 = string.Empty;

                if (reg.IsMatch(jsCrypt))
                {
                    string base64   = reg.Match(jsCrypt).Value.Split("'".ToCharArray()[0])[1];
                    string js       = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                    string[] lines  = getLines(js);

                    string var0 = lines[0].Split('=')[0];

                    string[] exps0 = lines[0].Substring(lines[0].IndexOf("=") + 1).Split('+');
                    string[] exps1 = lines[1].Substring(lines[1].IndexOf("=") + 1).Split('+');

                    foreach (string exp in exps0)
                        lin0 += evalExp(exp.Trim());

                    foreach (string exp in exps1)
                        lin1 += evalExp(exp.Trim(), var0, lin0);

                    headers.Add (new HttpHeader("Cookie", lin1));
                }
            }
            catch (Exception ex)
            {
                error = "HackSucuri.addSucuriHeaders -> " + ex.Message;
            }

            return (0 == error.Length);
        }


        //public static void passSucuri(string jsCrypt, ref Headers headers)
        //{
        //    Regex reg = new Regex("S='.+?'", RegexOptions.Singleline);

        //    string lin0 = string.Empty;
        //    string lin1 = string.Empty;

        //    if (reg.IsMatch(jsCrypt))
        //    {
        //        string base64 = reg.Match(jsCrypt).Value.Split("'".ToCharArray()[0])[1];
        //        string js = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        //        string[] lines = getLines(js);

        //        string var0 = lines[0].Split('=')[0];

        //        string[] exps0 = lines[0].Substring(lines[0].IndexOf("=") + 1).Split('+');
        //        string[] exps1 = lines[1].Substring(lines[1].IndexOf("=") + 1).Split('+');

        //        foreach (string exp in exps0)
        //            lin0 += evalExp(exp.Trim());

        //        foreach (string exp in exps1)
        //            lin1 += evalExp(exp.Trim(), var0, lin0);

        //        headers.Add(new Header("Cookie", lin1));
        //    }
        //}

        static string evalExp (string exp, string identifier = "", string change = "")
        {
            string val = string.Empty;

            if (!exp.StartsWith("'") && !exp.StartsWith("\""))
            {
                if (exp.StartsWith("String.fromCharCode"))
                {
                    string value = exp.Split("()".ToCharArray())[1];

                    val = value.StartsWith("0x") ?
                        val = ((char)int.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber)).ToString() :
                        val = ((char)int.Parse(value)).ToString();
                }
                else
                {
                    if (exp == identifier)
                        val = change;
                    else
                        throw new Exception("Comando desconocido");
                }
            }
            else
            {
                string[] parts = exp.Split('.');

                if (parts.Length > 1)
                {
                    string value = evalExp(parts[0]);
                    string cmd = parts[1];

                    if (cmd.StartsWith("charAt"))
                    {
                        int index = int.Parse(cmd.Split("()".ToCharArray())[1]);
                        val = value.Substring(index, 1);
                    }
                    else if (cmd.StartsWith("substr"))
                    {
                        int index = int.Parse(cmd.Split("(,)".ToCharArray())[1]);
                        int len = int.Parse(cmd.Split("(,)".ToCharArray())[2]);
                        val = value.Substring(index, len);
                    }
                    else if (cmd.StartsWith("slice"))
                    {
                        int index = int.Parse(cmd.Split("(,)".ToCharArray())[1]);
                        int len = int.Parse(cmd.Split("(,)".ToCharArray())[2]);
                        val = value.Substring(index, len - index);
                    }
                    else
                    {
                        throw new Exception("Comando desconocido");
                    }
                }
                else
                {
                    val = parts[0].Substring(1, parts[0].Length - 2);
                }
            }

            return val;
        }

        static string[] getLines (string js)
        {
            List<string> lines = new List<string>();
            string aux = string.Empty;
            bool text = false;

            for (int k = 0; k < js.Length; k++)
            {
                string c = js.Substring(k, 1);
                bool i = (c == "'" || c == "(" || c == ")" || c == "\"");

                if (i) text = !text;

                if (c == ";")
                {
                    if (!text)
                    {
                        lines.Add(aux);
                        aux = string.Empty;
                    }
                    else
                    {
                        aux += c;
                    }
                }
                else
                {
                    aux += c;
                }
            }

            return lines.ToArray();
        }
    }
}
