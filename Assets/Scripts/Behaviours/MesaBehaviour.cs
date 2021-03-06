﻿ using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.UI;

 public class MesaBehaviour : MonoBehaviour
 {
    float x,y;
    [Header("Animações do Baralho")]
    [SerializeField] float velocidadeAnimacao = 1f;
    [SerializeField] float altitude = 0 ;
    [SerializeField] float latitude = 0; 
    [Header("Configurações padrões")]
    [HideInInspector]
    public float distanciamentoCartasMaximo;
    public GameObject carta;
    private EventControllerBehaviour controller;
    //vida??? nao lembro mas provavelente sim k
    public GameObject Dano;
    public List<GameObject> cartas = new List<GameObject>();
    
    bool angularBaralho;
    void Start()
    {
        controller = transform.parent.GetComponent<EventControllerBehaviour>();
        distanciamentoCartasMaximo = 40;
    }

    void FixedUpdate()
    {
        if(angularBaralho)
            Angular();
    }

    public void SetRaycast(bool result)
    {
        foreach(var obj in cartas)
        {
            if (obj != null)
            obj.transform.GetChild(0).GetComponent<Image>().raycastTarget = result;
        }
    }
  
    public void SetAnimacao(float distanciamentoCartasMaximo) 
    {
        // formula que leva em conta um valor de distancia do ponto 0 qualquer (distanciamentoDeCartaMaximo), e a quantidade de vezes
        // em que essa distancia é dividida igualmente (Quantidade de cartas). Devolvendo a constante de distanciamento (Levando em conta 
        // a imparidade ou paridade da divisão.

        //constante de distanciamento
        distanciamentoCartasMaximo = (cartas.Count < 7) ? distanciamentoCartasMaximo : 175;
        float angulacaoConst = cartas.Count % 2 == 0f ? distanciamentoCartasMaximo / (float)(cartas.Count / 2) : distanciamentoCartasMaximo / (float)((cartas.Count - 1) / 2);
        //distancia inicial
        float concatenador = -distanciamentoCartasMaximo;
        int index = 0;
        if (cartas.Count == 1)
            concatenador = 0;
        foreach (var obj in cartas)
        {
            CartaNaMesa atributos = obj.transform.GetChild(0).GetComponent<CartaNaMesa>();
            //setando ID da carta em relação ao baralho
            atributos.PosicaoBaralho = index;
            // Setando posição da carta final e inicial 
            atributos.PosicaoInicial = obj.transform.localPosition;
            atributos.PosicaoFinal = new Vector2(concatenador * latitude,altitude);
            concatenador += angulacaoConst;
            index++;
        }
        angularBaralho = true;
        x = 0;
    }
     public void CriarCartaInicio(float ataq,float def,Sprite img,Evento ev,PassivaComulativa ef,AlvoPassiva alv,AudioClip[] aoMorrer)
     {
         GameObject objCarta = Instantiate(carta);
         objCarta.transform.localPosition = new Vector2(0,-20);
         objCarta.GetComponent<Image>().raycastTarget = false;
        CartaNaMesa obj = objCarta.transform.GetChild(0).GetComponent<CartaNaMesa>();
         objCarta.transform.SetParent(transform, false);
         obj.definirComeco(ataq,def,img,ef,ev,alv);
         obj.AoMorrer = aoMorrer;
         cartas.Add(objCarta);
         distanciamentoCartasMaximo += 20;
         //dispersar cartas na mesa
         SetAnimacao(distanciamentoCartasMaximo);
         switch(obj.AtivarPassivaQuando)
         {
            case Evento.CartaIniciada:
               controller.rodandoPassivasCampoProprio = !(obj.name == "CartaNaMesaInimigo");
               controller.RealizarPassivaEm(obj.Passiva,obj.Alvo,(objCarta.tag == "Player"),objCarta);
                break;
         }

         
        
    }  
    private void Angular() 
    {
        //função
        y = -x*x + 2*x;
        //velocidade da animação
        x += (velocidadeAnimacao * Time.deltaTime);
        //dado o fim da animação
        if (x >= 1)
        {
            angularBaralho = false;
            return;
        }
        //animando todas as cartas da mão
        //por meio do metodo vector.lerp
        foreach(var obj in cartas)
        {
            CartaNaMesa atributos = obj.transform.GetChild(0).GetComponent<CartaNaMesa>();
            obj.transform.localPosition = Vector2.Lerp(atributos.PosicaoInicial, atributos.PosicaoFinal, y);
        }
    }
 }
