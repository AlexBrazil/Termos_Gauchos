using System;
using System.Collections.Generic;


    // Para config.json
    [Serializable]
    public class GameConfig
    {
        public int tempoDoJogoMs;
        public int pontosPorAcerto;
        public int pontosPorErro;
        public int penalidadeTempoPorErroMs;
        public int bonusCenaSemErros;
        public int quantidadeDeCenas;
    }

    // Para cada item em palavras.json
    [Serializable]
    public class WordData
    {
        public string termo;
        public string significado;
        public string confundir;
    }

    // Para a lista completa de palavras.json
    [Serializable]
    public class WordList
    {
        public List<WordData> palavras;
    }
