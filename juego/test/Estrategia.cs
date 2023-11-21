
using SharpDX.DirectWrite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DeepSpace
{

	class Estrategia
	{

        //Calcula y retorna un texto con la
        //distancia del camino que existe entre el planeta del Bot y la raíz.

        public String Consulta1( ArbolGeneral<Planeta> arbol)
		{
			int cont = 0;
			//Recorrido para encontrar primero el planeta del BOT:
			Cola<ArbolGeneral<Planeta>> c = new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> aux;
			c.encolar(arbol);
			c.encolar(null);
			if (arbol.getDatoRaiz().EsPlanetaDeLaIA())
			{
				return cont.ToString();
			}
			
			while (!c.esVacia())
			{
				
				aux=c.desencolar();
				if(aux == null)
				{
					cont++;
					if (!c.esVacia())
					{
						c.encolar(null);
					}
				}
				else 
				{
					
					if (aux.getDatoRaiz().EsPlanetaDeLaIA())
					{
						break;
					}
					foreach(var hijo in aux.getHijos())
					{
						c.encolar(hijo);
					}
				}
					
				}
			return "-Consulta 1-\nDistancia del camino entre el planeta del Bot y la raiz: " + cont.ToString();
			
		}



        //Retorna un texto con el listado de los
        //planetas ubicados en todos los descendientes del nodo que contiene al planeta
		//del Bot.

        public String Consulta2( ArbolGeneral<Planeta> arbol)
		{
			string planetas = "";
            void preOrden(ArbolGeneral<Planeta> nodo)
            {
                if (nodo.getDatoRaiz().EsPlanetaDeLaIA())
                {
                    foreach (var hijo in nodo.getHijos())
                    {
						if (hijo.getDatoRaiz().EsPlanetaNeutral())
						{
							planetas += hijo.getDatoRaiz().Poblacion() + ",";
							foreach(var hijo2 in hijo.getHijos())
							{
                                planetas += hijo2.getDatoRaiz().Poblacion() + ",";
                            }
						}
						
                    }


                }

                foreach (var hijo in nodo.getHijos())
                {
                    preOrden(hijo); // Recorre los hijos de manera recursiva.
                }

            }

            preOrden(arbol);
            if (planetas.Length == 0)
            {
                return "\n-Consuta 2-\n El Planeta BOT no tiene descendientes";
            }
            else { return "\n-Consulta 2-\nDescendientes del planeta BOT: " + planetas; }
			

        }

		//Calcula y retorna en un texto la
		//población total y promedio por cada nivel del árbol.


		public String Consulta3(ArbolGeneral<Planeta> arbol)
		{
			Cola<ArbolGeneral<Planeta>> c = new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> aux;
			c.encolar(arbol);
			c.encolar(null);
			int poblacionT = 0; // poblacion total
			int cont = 0;  // para el nivel 
			string poblacionProm = ""; 
			float poblacionNivel = 0; 
			int nodos = 0; //para el promedio

			while (!c.esVacia())
			{

				aux = c.desencolar();
				if (aux == null)
				{

					poblacionProm+="Nivel " + cont + " --> Problacion promedio: " + (poblacionNivel / nodos)+"\n";
					cont++;
					poblacionNivel = 0;
					nodos = 0;
					if (!c.esVacia())
					{
						c.encolar(null);
					}
				}
				else
				{
					nodos++;
					poblacionNivel+= aux.getDatoRaiz().Poblacion();
                    poblacionT += aux.getDatoRaiz().Poblacion();
					foreach (var hijo in aux.getHijos())
					{
						c.encolar(hijo);
					}
				}

			}
			return "\n-------------\n-Consulta 3-\nPoblacion total: " + poblacionT.ToString()+"\n "+poblacionProm ;
        }

        public Movimiento CalcularMovimiento(ArbolGeneral<Planeta> arbol)
        {

            List<Planeta> Ataque = CaminoEstrategiaBot(arbol);
            List<Planeta> CaminoHaciaBot = CaminoABot(arbol);
            List<Planeta> CaminoHaciaJugador = CaminoAJugador(arbol);
            List<Planeta> CaminoBotHaciaJugador = CaminoBotAJugador(CaminoHaciaBot, CaminoHaciaJugador);

            if (CaminoBotHaciaJugador[1].EsPlanetaDelJugador())
            {

                CaminoBotHaciaJugador = EstrategiaAtaque(Ataque, CaminoHaciaJugador);
                Ataque = CaminoBotAJugador(CaminoHaciaBot, CaminoHaciaJugador);

                if (Ataque[0].Poblacion() > Ataque[1].Poblacion())
                {
                    CaminoBotHaciaJugador = CaminoBotAJugador(CaminoHaciaBot, CaminoHaciaJugador);
                }
            }

            Movimiento ataque = new Movimiento(CaminoBotHaciaJugador[0], CaminoBotHaciaJugador[1]);

            return ataque;
        }


        // Caminos

        //Construir el camino desde la raiz hacia el bot.
        private List<Planeta> CaminoABot(ArbolGeneral<Planeta> arbol, List<Planeta> caminoHaciaBot = null)
        {
            if (caminoHaciaBot == null)
            {
                caminoHaciaBot = new List<Planeta>();
            }

            caminoHaciaBot.Add(arbol.getDatoRaiz());

            if (arbol.getDatoRaiz().EsPlanetaDeLaIA())
            {
                return caminoHaciaBot;
            }

            foreach (var hijo in arbol.getHijos())
            {
                List<Planeta> resultado = CaminoABot(hijo, caminoHaciaBot);

                if (resultado != null)
                {
                    return resultado;
                }

                caminoHaciaBot.RemoveAt(caminoHaciaBot.Count - 1);
            }

            return null;
        }

        private List<Planeta> CaminoAJugador(ArbolGeneral<Planeta> arbol, List<Planeta> caminoHaciaJugador = null)
        {

            if (caminoHaciaJugador == null)
            {
                caminoHaciaJugador = new List<Planeta>();
            }

            caminoHaciaJugador.Add(arbol.getDatoRaiz());

            if (arbol.getDatoRaiz().EsPlanetaDelJugador())
            {
                return caminoHaciaJugador;
            }
            foreach (var hijo in arbol.getHijos())
            {
                List<Planeta> resultado = CaminoAJugador(hijo, caminoHaciaJugador);

                if (resultado != null)
                {
                    return resultado;
                }

                caminoHaciaJugador.RemoveAt(caminoHaciaJugador.Count - 1);
            }

            return null;
        }

        

        private List<Planeta> CaminoBotAJugador(List<Planeta> CaminoHaciaBot, List<Planeta> CaminoHaciaJugador)
        {
            //Listas para la clasificacion de los planetas en el camino.

            List<Planeta> PlanetasDelBot = new List<Planeta>();

            List<Planeta> PlanetasNeutro = new List<Planeta>();

            List<Planeta> PlanetasDelJugador = new List<Planeta>();

            //Ancestro comun entre bot y jugador para unir los caminos
            List<Planeta> AncestrosComun = new List<Planeta>();

            //Lista del camino final.
            List<Planeta> CaminoBotHaciaJugador = new List<Planeta>();
            //Planeta ancestro comun.
            Planeta ancestroComun;

            
            bool existe = false;

            //Itero por cada elemento de ambas lista buscando el ancestro comun
            for (int i = 0; i < CaminoHaciaBot.Count && i < CaminoHaciaJugador.Count; i++)
            {
                if (CaminoHaciaBot[i] == CaminoHaciaJugador[i]) 
                {
                    AncestrosComun.Add(CaminoHaciaBot[i]); 
                }
            }
            ancestroComun = AncestrosComun[AncestrosComun.Count - 1];

            // Construir la 1era parte del camino desde el último planeta del bot hasta el ancestro común
            for (int i = CaminoHaciaBot.Count - 1; i >= 0; i--)
            {
                CaminoBotHaciaJugador.Add(CaminoHaciaBot[i]);
                if (CaminoHaciaBot[i] == ancestroComun)
                {
                    break;
                }
            }
            //Agregar la 2da parte del camino: desde el ancestro común hasta el final del camino del jugador
            foreach (var c in CaminoHaciaJugador)
            {
                if (existe) //primero esta en false, luego en el siguiente condicional se pone true
                {
                    CaminoBotHaciaJugador.Add(c);
                }

                if (c == ancestroComun)
                {
                    existe = true;
                }
            }
            //Clasificacion de los planetas
            foreach (var c in CaminoBotHaciaJugador)
            {
                
                if (c.EsPlanetaDeLaIA())
                {
                    PlanetasDelBot.Add(c);
                }
                else if (c.EsPlanetaDelJugador())
                {
                    PlanetasDelJugador.Add(c);
                }
                else if (c.EsPlanetaNeutral())
                {
                    PlanetasNeutro.Add(c);
                }
            }
            CaminoBotHaciaJugador.Clear();

            CaminoBotHaciaJugador.Add(PlanetasDelBot[PlanetasDelBot.Count - 1]);

            foreach (var c in PlanetasNeutro)
            {
                CaminoBotHaciaJugador.Add(c);
            }

            CaminoBotHaciaJugador.Add(PlanetasDelJugador[0]);
            

            return CaminoBotHaciaJugador;
        }


        // Ataque
        private void AtaqueBot(ArbolGeneral<Planeta> arbol, List<Planeta> Ataque)
        {

            if (arbol.getDatoRaiz().EsPlanetaDeLaIA())
            {
                Ataque.Add(arbol.getDatoRaiz());
            }

            foreach (var hijo in arbol.getHijos())
            {
                AtaqueBot(hijo, Ataque);
            }
        }

        private void Ordenar(ref List<Planeta> datosP)
        {
            int n = datosP.Count;
            for (int i = 0; i < (n - 1); i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (datosP[i].Poblacion() > datosP[j].Poblacion())
                    {
                        Planeta swap = datosP[i];
                        datosP[i] = datosP[j];
                        datosP[j] = swap;
                    }
                }
            }
        }

        private List<Planeta> CaminoEstrategiaBot(ArbolGeneral<Planeta> arbol)
        {
            List<Planeta> Ataque = new List<Planeta>();
            List<Planeta> CaminoHaciaBot = new List<Planeta>();
            Planeta Max = null;

            // Función interna que realiza la búsqueda del camino y actualiza Maximo
            bool BuscarCamino(ArbolGeneral<Planeta> nodoActual)
            {
                CaminoHaciaBot.Add(nodoActual.getDatoRaiz());

                if (nodoActual.getDatoRaiz() == Max)
                {
                    return true;
                }
                else
                {
                    foreach (var hijo in nodoActual.getHijos())
                    {
                        if (BuscarCamino(hijo))
                        {
                            return true;
                        }
                        CaminoHaciaBot.RemoveAt(CaminoHaciaBot.Count - 1);
                    }
                }
                return false;
            }

            // Lleno la lista de Ataque con los planetas de la IA en el árbol
            AtaqueBot(arbol, Ataque);

            // Ordeno la lista de Ataque por la población de los planetas
            Ordenar(ref Ataque);

            // Obtengo el planeta con la población máxima de la lista de Ataque
            Max = Ataque[Ataque.Count - 1];

            // Llamo a la función interna para realizar la búsqueda del camino
            BuscarCamino(arbol);

            return CaminoHaciaBot;
        }

        private List<Planeta> EstrategiaAtaque(List<Planeta> Ataque, List<Planeta> CaminoHaciaJugador)
        {

            List<Planeta> AncestrosComun = new List<Planeta>();

            List<Planeta> CaminoBotHaciaJugador = new List<Planeta>();

            Planeta ancestroComun;

            bool existe = false;

            for (int i = 0; i < Ataque.Count && i < CaminoHaciaJugador.Count; i++)
            {
                if (Ataque.Contains(CaminoHaciaJugador[i]))
                {
                    AncestrosComun.Add(CaminoHaciaJugador[i]);
                }
            }
            ancestroComun = AncestrosComun[AncestrosComun.Count - 1];

            for (int i = Ataque.Count - 1; i >= 0; i--)
            {
                CaminoBotHaciaJugador.Add(Ataque[i]);
                if (Ataque[i] == ancestroComun)
                {
                    break;
                }
            }
            foreach (var c in CaminoHaciaJugador)
            {
                if (existe)
                {
                    CaminoBotHaciaJugador.Add(c);
                }

                if (c == ancestroComun)
                {
                    existe = true;
                }
            }
            return CaminoBotHaciaJugador;
        }
    }
}
