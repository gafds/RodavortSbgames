﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime;

public class Mao : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler ,IPointerExitHandler,IPointerEnterHandler
{
    [Header("Animações do Baralho")]
    [SerializeField] float velocidadeAnimacao = 1f;
    [SerializeField] float distancia = 1;
    [SerializeField] float indiceAngulacao = 12;
    [SerializeField] float altitude = -290 ;
    [SerializeField] float latitude = 0; 
    [Header("Configurações de Audio")]
    public AudioSource som;
    public AudioClip[] audios;
    [Header("Objetos")]
    public Text vidaPlayer;
    public Text goldPlayer;

    public GameObject carta,Seta,SetaEfeito,Dano;
    public List<GameObject> mao = new List<GameObject>();
    float x,y;
    //PlayerId playerid;
    //NetworkIdentity ntwrkid;
    [Header("VIDA")]
    public float vida;
    private int gold;
    public float distanciamentoCartasMaximo;
    GraphicRaycaster raycast;
    EventSystem input;
    Exibicao exibir;
    Animator OutPut;
    GameObject CartaAtual,AtaqueNoInimigo,seta,dano,setaEfeito;
    Carta cartaEfeito;
    List<RaycastResult> resultados;
    PointerEventData cursor;
    bool animarBaralho;
    bool entrar,isOnEffect;


    #region ClickInput
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(Input.touchCount < 2)
        {
            //ntwrkid = NetworkClient.connection.identity;
            //playerid = ntwrkid.GetComponent<PlayerId>();
            CartaAtual = eventData.pointerCurrentRaycast.gameObject;

           if(CartaAtual != null)// && playerid.isplayer2)
           {
             switch(EventControllerBehaviour.turno)
                {
                    case EventControllerBehaviour.Turnos.TurnoEscolhaP1:

                        if(CartaAtual.name == "Carta(Clone)")
                            PuxarCartaBaralho();      

                        goto case EventControllerBehaviour.Turnos.TurnoAtaqueP1;

                    case EventControllerBehaviour.Turnos.TurnoAtaqueP1:

                        if(CartaAtual.name == "CartaNaMesa")
                           PuxarCartaMesa();                                                        
                        break;
                }
           }  
           //inverti sem querer agr ja era
           else if(CartaAtual != null)// && !playerid.isplayer2)
           {
             switch(EventControllerBehaviour.turno)
                {
                    case EventControllerBehaviour.Turnos.TurnoEscolhaP2:
                    
                        if(CartaAtual.name == "Carta(Clone)")
                            PuxarCartaBaralho();   

                        goto case EventControllerBehaviour.Turnos.TurnoAtaqueP2;
                        
                    case EventControllerBehaviour.Turnos.TurnoAtaqueP2:

                        if(CartaAtual.name == "CartaNaMesa")
                            PuxarCartaMesa();                                   
                        break;
                }
           }      
          
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
       if(CartaAtual != null && CartaAtual.name == "segurado" )
            CartaAtual.transform.position = Input.mousePosition ;    
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        if (CartaAtual != null  && CartaAtual.name == "segurado")
        {
         mao.Insert(CartaAtual.GetComponent<Carta>().PosicaoBaralho,CartaAtual);
         distanciamentoCartasMaximo +=20;
         CartaAtual.name = "Carta(Clone)";
         SetRaycast(true);
        }
         SetAnimacao(distanciamentoCartasMaximo);
         OutPut.SetBool("MouseNaCarta",false);
    }
    public void OnPointerEnter(PointerEventData eventData) 
    {   
        CartaAtual = eventData.pointerCurrentRaycast.gameObject;
        entrar = true;      
        if (CartaAtual != null && CartaAtual.name == "Carta(Clone)")
        {
            som.PlayOneShot(audios[0]);
            OutPut.SetBool("MouseNaCarta",true);
            exibir.SetAtributos(CartaAtual.GetComponent<Carta>().Nome,CartaAtual.GetComponent<Carta>().Descricao,CartaAtual.GetComponent<Carta>().Valor.ToString(),CartaAtual.GetComponent<Carta>().Ataque.ToString(),CartaAtual.GetComponent<Carta>().Defesa.ToString(),CartaAtual.GetComponent<Carta>().Imagem);
            SetAnimacao(distanciamentoCartasMaximo);
            SetPosicao(eventData.pointerCurrentRaycast.gameObject,30,0);
        }
    } 
    public void OnEndDrag(PointerEventData eventData)
    {
        //ntwrkid = NetworkClient.connection.identity;
        //playerid = ntwrkid.GetComponent<PlayerId>();
        cursor.position = Input.mousePosition;
        resultados = new List<RaycastResult>();
        raycast.Raycast(cursor, resultados);
        //CartaAtual = eventData.pointerCurrentRaycast.gameObject;
        // if(playerid.isplayer2)
        // {
        //     switch(EventControllerBehaviour.turno)
        //     {
        //         case EventControllerBehaviour.Turnos.TurnoEscolhaP1:

        //             if( resultados.Count > 1 && resultados[resultados.Count - 1].gameObject.name == "CampoAmigo")
        //                 ColocarCartaNaMesa();

        //             else if (CartaAtual != null && CartaAtual.name == "segurado")
        //                 RejeitarCarta();      

        //         goto case EventControllerBehaviour.Turnos.TurnoAtaqueP1;

        //         case EventControllerBehaviour.Turnos.TurnoAtaqueP1:

        //             if(resultados.Count > 0 )
        //                Atacar();

        //         break;
        //     }
        // }
        // else{
        //     switch(EventControllerBehaviour.turno)
        //     {
        //         case EventControllerBehaviour.Turnos.TurnoEscolhaP2:

        //             if( resultados.Count > 1 && resultados[resultados.Count - 1].gameObject.name == "CampoAmigo")
        //                 ColocarCartaNaMesa();

        //             else if (CartaAtual != null && CartaAtual.name == "segurado")
        //                 RejeitarCarta();

        //         goto case EventControllerBehaviour.Turnos.TurnoAtaqueP2;

        //         case EventControllerBehaviour.Turnos.TurnoAtaqueP2:
                
        //             if(resultados.Count > 0 )
        //                 Atacar();

        //         break;
        //      }
             
        // }     
        CartaAtual = null;
    }
    
    public void Mouse()
    {
        cursor.position = Input.mousePosition;
        resultados = new List<RaycastResult>();
        raycast.Raycast(cursor, resultados);
        if (resultados.Count != 0)
        {
            if (resultados[0].gameObject.name == "Carta(Clone)")
            {
                if (resultados[0].gameObject != CartaAtual && entrar)
                {
                    SetAnimacao(distanciamentoCartasMaximo);
                    entrar = false;
                }
                else if (resultados[0].gameObject != CartaAtual)
                {
                    som.PlayOneShot(audios[0]);
                    entrar = true;
                    OutPut.SetBool("MouseNaCarta",true);
                    CartaAtual = resultados[0].gameObject;
                    Carta atributos = CartaAtual.GetComponent<Carta>();
                    exibir.SetAtributos(atributos.Nome,atributos.Descricao,atributos.Valor.ToString(),atributos.Ataque.ToString(),atributos.Defesa.ToString(),atributos.Imagem);
                    SetAnimacao(distanciamentoCartasMaximo);
                    SetPosicao(resultados[0].gameObject,30,0);
                }
            }
            else if (entrar && resultados[0].gameObject)
            {
		        SetAnimacao(distanciamentoCartasMaximo);
                OutPut.SetBool("MouseNaCarta",false);
                entrar = false;
            }    
        }

    }
    #endregion
    #region FisicasCarta
    private void SetPosicao(GameObject Carta, float longitude , float latitude)
    {
        Carta atributos =  Carta.GetComponentInParent<Carta>();
        atributos.PosicaoFinal = new Vector2(atributos.PosicaoFinal.x + latitude, atributos.PosicaoFinal.y + longitude);
        atributos.AngulacaoFinal = Vector3.zero;
    }
    public void SetAnimacao(float distanciamentoCartasMaximo)
    {
        // formula que leva em conta um valor de distancia do ponto 0 qualquer (distanciamentoDeCartaMaximo), e a quantidade de vezes
        // em que essa distancia é dividida igualmente (Quantidade de cartas). Devolvendo a constante de distanciamento (Levando em conta 
        // a imparidade ou paridade da divisão.

        //constante de distanciamento
        float angulacaoConst = mao.Count % 2 == 0f ? distanciamentoCartasMaximo / (float)(mao.Count / 2) : distanciamentoCartasMaximo / (float)((mao.Count - 1) / 2);
        //distancia inicial
        float concatenador = -distanciamentoCartasMaximo;
        int index = 0;
        if (mao.Count == 1)
            concatenador = 0;
        foreach (var obj in mao)
        {
            //setando ID da carta em relação ao baralho
            obj.GetComponent<Carta>().PosicaoBaralho = index;
            // Setando posição da carta final e inicial 
            obj.GetComponent<Carta>().PosicaoInicial = obj.transform.localPosition;
            obj.GetComponent<Carta>().PosicaoFinal = new Vector2(concatenador * distancia + latitude, -Mathf.Abs(concatenador) / 5 + altitude);
            // Setando a Angulação final e inicial
            obj.GetComponent<Carta>().AngulacaoInicial = obj.transform.GetChild(0).eulerAngles;
            obj.GetComponent<Carta>().AngulacaoFinal = new Vector3(0, 0, (-concatenador / indiceAngulacao));
            if (concatenador == 0 || concatenador == distanciamentoCartasMaximo || concatenador == -distanciamentoCartasMaximo)
                obj.GetComponent<Carta>().PosicaoFinal = new Vector3(concatenador * distancia + latitude, -Mathf.Abs(concatenador) / 5 + altitude);
            concatenador += angulacaoConst;
            obj.transform.SetSiblingIndex(index);
            index++;
        }
        animarBaralho = true;
        x = 0;
    }

    //Seta atributos da carta para realização da animação posteriormente
    //atributos simulam o distanciamento e angulação de um baralho em mãos 


    // executa a animação de movimentação do baralho do ponto inicial ao final ja setado com a suavização
    // dada pela função final ja setado com a suavização dada pela função "f(x)= -x² + 2x" 
    private void Angular()
    {
        //função
        y = -x * x + 2 * x;
        //velocidade da animação
        x += (velocidadeAnimacao * Time.deltaTime);
        //dado o fim da animação
        if (x >= 1)
        {
            animarBaralho = false;
            return;
        }
        //animando todas as cartas da mão
        //por meio do metodo vector.lerp
        foreach (var obj in mao)
        {
            Carta atributos = obj.GetComponent<Carta>();
            atributos.AngulacaoInicial = (atributos.AngulacaoInicial.z > 180) ? atributos.AngulacaoInicial - Vector3.forward * 360 : atributos.AngulacaoInicial;
            atributos.AngulacaoFinal = (atributos.AngulacaoFinal.z > 180) ? atributos.AngulacaoFinal - Vector3.forward * 360 : atributos.AngulacaoFinal;
            obj.transform.localPosition = Vector2.Lerp(atributos.PosicaoInicial, atributos.PosicaoFinal, y);
            obj.transform.GetChild(0).eulerAngles = Vector3.Lerp(atributos.AngulacaoInicial, atributos.AngulacaoFinal, y);
        }

    }
    #region Repeticoes    
    public void CriarCartaInicio(int id)
    {
        GameObject objCarta = Instantiate(carta);
        objCarta.GetComponent<Carta>().Constructor(id);
        objCarta.transform.SetParent(transform.GetChild(4), false);
        mao.Add(objCarta);
        distanciamentoCartasMaximo += 20;
        SetAnimacaoInicial(distanciamentoCartasMaximo);
    }  
    
    //Faz o mesmo que o SetAnimacao, porem tem uma animação diferenciada.
    //Feita com o unico intuito de ser rodada apenas no inicio (animação de receber as cartas iniciais).
    public void SetAnimacaoInicial(float distanciamentoCartasMaximo)
    {
        float angulacaoConst = mao.Count % 2 == 0f ? distanciamentoCartasMaximo / (float)(mao.Count / 2) : distanciamentoCartasMaximo / (float)((mao.Count - 1) / 2);     
        float concatenador = -distanciamentoCartasMaximo;
         if (mao.Count == 1)
             return;           
         foreach(var obj in mao)
        {
            Carta atributos = obj.GetComponent<Carta>();
            atributos.PosicaoInicial = obj.transform.localPosition - Vector3.up * 450;
            atributos.PosicaoFinal = new Vector2(concatenador * distancia + latitude, -Mathf.Abs(concatenador) / 5 + altitude);     
            atributos.AngulacaoFinal = new Vector3(0, 0, (-concatenador  / indiceAngulacao) );
            concatenador += angulacaoConst;
            obj.transform.SetSiblingIndex(-1);
        }
       animarBaralho = true;
        x = 0;     
    }
    #endregion
    #endregion

    #region configuracoes
    public void Audio(int numero)
    {
        som.PlayOneShot(audios[numero]);
    }
    #endregion


    public void SetRaycast(bool result)
    {
        foreach(var obj in mao)
        {
            obj.GetComponent<Image>().raycastTarget = result;
        }
    }
    public void CriarCarta(int id)
    {
        GameObject objCarta = Instantiate(carta);
        objCarta.GetComponent<Carta>().Constructor(id);
        objCarta.transform.SetParent(transform.GetChild(4),false);
        objCarta.transform.localPosition += new Vector3(600, -290);  
        mao.Add(objCarta);
        distanciamentoCartasMaximo += 20;
        objCarta.GetComponent<Carta>().AngulacaoFinal = new Vector3(0, -90,-55);
        SetAnimacao(distanciamentoCartasMaximo);
    }

        void Awake()
        {
        mao = new List<GameObject>();  
        OutPut = transform.GetChild(5).GetComponent<Animator>();
        exibir = transform.GetChild(5).GetComponent<Exibicao>();
        vidaPlayer.text = vida + "/" + vida;

        cursor = new PointerEventData(input);
        resultados = new List<RaycastResult>();
        raycast = GetComponent<GraphicRaycaster>();
        input = GetComponent<EventSystem>();

        }
    void FixedUpdate()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR_WIN
            Mouse();
        #endif
        if (animarBaralho)
        {
            Angular();
        }
    }

    IEnumerator DarDano(GameObject obj)
 {
        yield return new WaitForSeconds(0.40f);
        if (obj)
        {
            obj.GetComponent<CartaNaMesa>().Defesa -= AtaqueNoInimigo.GetComponent<CartaNaMesa>().Ataque;
            AtaqueNoInimigo.GetComponent<CartaNaMesa>().Defesa -= obj.GetComponent<CartaNaMesa>().Ataque;
            som.PlayOneShot(audios[2]);
            AtaqueNoInimigo = null;
        }
    }
    IEnumerator DarDanoInimigo(GameObject obj) 
    {

        yield return new WaitForSeconds(0.40f);
        dano = Instantiate(Dano);
        dano.transform.SetParent(transform.GetChild(4), false);
        dano.transform.localPosition = obj.transform.localPosition + Vector3.up * 50 + Vector3.left * 30;
        dano.transform.localScale = new Vector3(3, 3);
        dano.GetComponent<Text>().text += AtaqueNoInimigo.GetComponent<CartaNaMesa>().Ataque.ToString();
        GetComponent<PlayerAdversario>().PerderVida(AtaqueNoInimigo.GetComponent<CartaNaMesa>().Ataque);
        som.PlayOneShot(audios[2]);
        AtaqueNoInimigo = null;
    }
    
    public void TirarVidaPlayer(float dano) 
    {
        vida -= dano;
        vidaPlayer.text = vida + "/15";
    }
    public void SetarGold(int goldMax) 
    {
        gold = goldMax;
        goldPlayer.text = string.Format("{0}/{1}",gold,goldMax);
       
    }
    
void Atacar(){
    foreach(var obj in resultados)
    {                                  
        // carta efeito
        if(isOnEffect && (obj.gameObject.name == "CartaNaMesaInimigo"|| obj.gameObject.name == "CartaNaMesa"))
        {
            int barra = goldPlayer.text.IndexOf('/');
            goldPlayer.text = (int.Parse(goldPlayer.text.Substring(0, barra)) - cartaEfeito.Valor).ToString() +"/" + EventControllerBehaviour.ouroMaximo;
            //playerid.CmdAtualizarGold(goldPlayer.text);
            this.gameObject.GetComponent<EventControllerBehaviour>().RealizarPassivaEm(cartaEfeito.Passiva,obj.gameObject.transform.parent.gameObject,true,null);
            //playerid.CmdTirarCartaBaralho(cartaEfeito.PosicaoBaralho);
            //carta ativo não referenciam objetos realizadores
           // playerid.CmdEfeitoRealizado(!(obj.gameObject.name == "CartaNaMesa"),obj.gameObject.GetComponent<CartaNaMesa>().PosicaoBaralho,cartaEfeito.Passiva.quantidade,(int)cartaEfeito.Passiva.efeito);
            SetRaycast(true);
            isOnEffect = false;
           // cartaEfeito = null;
        }
        //atacar carta
        if (obj.gameObject.name == "CartaNaMesaInimigo" && AtaqueNoInimigo)
        {                     
            CartaNaMesa refCard = AtaqueNoInimigo.GetComponent<CartaNaMesa>(); 
            if(refCard.QuantidadeAtaque > 0)
            {                                   
                //playerid.CmdAtacarCarta(refCard.PosicaoBaralho,obj.gameObject.GetComponent<CartaNaMesa>().PosicaoBaralho);
                AtaqueNoInimigo.transform.parent.GetComponent<Animator>().SetTrigger("Atacar");
                GameObject inimigo = obj.gameObject.transform.parent.gameObject;
                CartaNaMesa enemyRefCard = obj.gameObject.transform.GetComponent<CartaNaMesa>();
                // passiva do inimigo quando atacado se inicia primeiro do que quando a carta ataca primeiro
                switch(enemyRefCard.AtivarPassivaQuando)
                {
                     case Evento.CartaRecebeuDano:
                        if(enemyRefCard.Alvo != AlvoPassiva.CartaQueAtacou)
                            this.gameObject.GetComponent<EventControllerBehaviour>().RealizarPassivaEm(enemyRefCard.Passiva,enemyRefCard.Alvo,false,inimigo);
                        else
                            this.gameObject.GetComponent<EventControllerBehaviour>().RealizarPassivaEm(enemyRefCard.Passiva,AtaqueNoInimigo.transform.parent.gameObject,false,inimigo);                              
                     break;
                }
                switch(refCard.AtivarPassivaQuando)
                {
                    case Evento.CartaAtaque:

                        if(refCard.Alvo != AlvoPassiva.CartaAtacada)
                            this.gameObject.GetComponent<EventControllerBehaviour>().RealizarPassivaEm(refCard.Passiva,refCard.Alvo,true,AtaqueNoInimigo.transform.parent.gameObject);
                        else
                            this.gameObject.GetComponent<EventControllerBehaviour>().RealizarPassivaEm(refCard.Passiva,inimigo,true,AtaqueNoInimigo.transform.parent.gameObject);

                    break;
                }
                
                refCard.QuantidadeAtaque--;
                
                StartCoroutine(DarDano(obj.gameObject));
            }
        break;
        }
    //atacar player inimigo
        else if (obj.gameObject.name == "CampoInimigo" && obj.gameObject.GetComponent<MesaBehaviour>().cartas.Count == 0 && AtaqueNoInimigo)
        {  
            
            CartaNaMesa refCard = AtaqueNoInimigo.GetComponent<CartaNaMesa>(); 
        if(refCard.QuantidadeAtaque > 0)
            {                                                   
                AtaqueNoInimigo.transform.parent.GetComponent<Animator>().SetTrigger("Atacar");
                //playerid.CmdAtacarPlayer(refCard.PosicaoBaralho);
                refCard.QuantidadeAtaque--; 
                StartCoroutine(DarDanoInimigo(obj.gameObject));
            }
        }
} 
}
void ColocarCartaNaMesa()
{
     for(int i=0;i < resultados.Count - 1;i++)
    {
        switch(resultados[i].gameObject.name)
        {
            case "segurado":

                int barra = goldPlayer.text.IndexOf('/');
                if (resultados[i].gameObject.GetComponent<Carta>().Valor <= int.Parse(goldPlayer.text.Substring(0, barra)))
                {                      
                        //tirar carta mao****
                    //playerid.CmdTirarCartaBaralho(resultados[i].gameObject.GetComponent<Carta>().PosicaoBaralho);
                    //exibir pro player adversario que foi tirada uma carta
                    //playerid.CmdColocarCartaBaralho(resultados[i].gameObject.GetComponent<Carta>().Id);
                    goldPlayer.text = (int.Parse(goldPlayer.text.Substring(0, barra)) - resultados[i].gameObject.GetComponent<Carta>().Valor).ToString() +"/" + EventControllerBehaviour.ouroMaximo;
                    //playerid.CmdAtualizarGold(goldPlayer.text);
                    Carta atributos = resultados[i].gameObject.GetComponent<Carta>();
                    Card refCard =Resources.Load<Card>("InformacoesCartas/" + atributos.Id);
                    resultados[resultados.Count - 1].gameObject.GetComponent<MesaBehaviour>().CriarCartaInicio(atributos.Ataque, atributos.Defesa, atributos.Imagem,atributos.AtivarPassivaQuando,atributos.Passiva,atributos.Alvo,refCard.SomEmMorte);
                    DestruirCartaBaralho(resultados[i].gameObject);         
                    //som de entrada da carta individual
                    if(refCard.SomNaEntrada != null && refCard.SomNaEntrada.Length > 0)
                        som.PlayOneShot(refCard.SomNaEntrada[Random.Range(0,refCard.SomNaEntrada.Length)]);
                    //som padrão de entrada
                    som.PlayOneShot(audios[1]);
                    SetRaycast(true);
                }
                else
                {

                    mao.Insert(resultados[i].gameObject.GetComponent<Carta>().PosicaoBaralho,resultados[i].gameObject);
                    distanciamentoCartasMaximo += 20;
                    SetAnimacao(distanciamentoCartasMaximo);
                    //colocar um audio de negação
                    som.PlayOneShot(audios[2]);
                    SetRaycast(true);
                    resultados[i].gameObject.name = "Carta(Clone)";
                }
            break;
        }
    }
}
    void RejeitarCarta()
    {
        mao.Insert(CartaAtual.GetComponent<Carta>().PosicaoBaralho,CartaAtual);
        distanciamentoCartasMaximo +=20;
        CartaAtual.name = "Carta(Clone)";
        SetRaycast(true);
        SetAnimacao(distanciamentoCartasMaximo);         
    }
    void PuxarCartaBaralho()
    {       
        x=1;
        OutPut.SetBool("MouseNaCarta",false);
        mao.RemoveAt(CartaAtual.GetComponent<Carta>().PosicaoBaralho);
        SetRaycast(false);
        distanciamentoCartasMaximo -=20;
        CartaAtual.name = "segurado";
        SetAnimacao(distanciamentoCartasMaximo);

        if(CartaAtual.GetComponent<Carta>().IsAtivo)
        {
            int barra = goldPlayer.text.IndexOf('/');
            if (CartaAtual.gameObject.GetComponent<Carta>().Valor <= int.Parse(goldPlayer.text.Substring(0, barra)))
                {                      
                    //playerid.CmdAtualizarGold(goldPlayer.text);
                    cartaEfeito = CartaAtual.GetComponent<Carta>();
                    DestruirCartaBaralho(CartaAtual);
                    InstanciarSeta();
                    isOnEffect = true;
                }
        }
    }
    void OnApplicationQuit()
    {
        //playerid.CmdStopHost();
    }
    void PuxarCartaMesa()
    {
        AtaqueNoInimigo = CartaAtual;
        CartaNaMesa refCard = AtaqueNoInimigo.GetComponent<CartaNaMesa>();
            if(refCard.PodeAtacar)
            {
                seta = Instantiate(Seta);
                seta.transform.SetParent(transform.GetChild(4),false);
                seta.transform.localPosition = CartaAtual.transform.parent.localPosition - new Vector3(10,80);
            }                                          
    }

    void InstanciarSeta()
    {
        setaEfeito = Instantiate(SetaEfeito);
        setaEfeito.transform.SetParent(transform.GetChild(4),false);
        setaEfeito.transform.localPosition = Vector2.up * -200;
    }
    public void DestruirCartaBaralho(GameObject carta)
    {
        carta.name = "Destruido";
        carta.gameObject.GetComponent<Image>().raycastTarget = false;
        carta.gameObject.GetComponent<Animator>().SetBool("Destruir", true);
    }
    public void EfeitoCancelado()
    {
        if(isOnEffect)
        {
            CriarCarta(int.Parse(cartaEfeito.Id));     
            SetRaycast(true);
            cartaEfeito = null;
        }
    }
}