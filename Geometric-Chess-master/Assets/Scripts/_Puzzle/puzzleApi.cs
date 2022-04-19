using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using SimpleJSON;
using System.Text;

namespace com.impactional.chess
{
    public class puzzleApi : MonoBehaviour
    {
        public puzzleChessBoard gridPuz;

        public void fetchPuzzle() => StartCoroutine(fetchPuzzle_coroutine());

        IEnumerator fetchPuzzle_coroutine()
        {
            Debug.Log("fetching puzzle");
            string url = "https://api.chess.com/pub/puzzle/random";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);

                }
                else
                {
                    Debug.Log(request.downloadHandler.text);

                    JSONNode node = JSON.Parse(request.downloadHandler.text);


                    //{ "title":"Wolfgang Wednesday",
                    //        "url":"https://www.chess.com/forum/view/daily-puzzles/6-23-2021-wolfgang-wednesday",
                    //        "publish_time":1624431600,
                    //        "fen":"2r3k1/pb3ppp/8/5Q2/8/1P1nqNP1/P3P2P/4RB1K w - - 4 26",
                    //        "pgn":"[Event \"Halle DSV 07th\"]
                    //        \r\n[Site \"Halle GDR\"]\r\n
                    //        [Date \"1984.06.22\"]\r\n
                    //        [Round \"11\"]\r\n
                    //        [White \"Wolfgang Uhlmann\"]\r\n
                    //        [Black \"Thomas Paehtz Sr\"]\r\n
                    //        [Result \"*\"]\r\n
                    //        [WhiteElo \"2505\"]\r\n
                    //        [BlackElo \"2360\"]\r\n
                    //        [FEN \"2r3k1/pb3ppp/8/5Q2/8/1P1nqNP1/P3P2P/4RB1K w - - 4 26\"]\r\n
                    //        \r\n
                    //        26.Qxc8+ Bxc8 27.exd3 Qxf3+ 28.Bg2 1-0",
                    //        "image":"https://www.chess.com/dynboard?fen=2r3k1/pb3ppp/8/5Q2/8/1P1nqNP1/P3P2P/4RB1K%20w%20-%20-%204%2026&size=2"}

                    //Debug.Log(node["fen"].ToString());

                    string fenPos = node["fen"].ToString();

                    gridPuz.fenPosition = fenPos.Substring(1, fenPos.Length - 2);

                    string pgn = node["pgn"].ToString();
                    
                    gridPuz.resultPgn = fetchPuzzleSoulution(pgn);

                  
                    
                   

                  


                    Debug.Log(gridPuz.fenPosition);


                    gridPuz.SpawnAllPieces();

                    //gridPuz.ConvertResultStringToPuzzleMoveList();

                }
            }
        }


        public string fetchPuzzleSoulution( string pgnString)
        {
            string solution = "" ;

            //[Date \"????.??.??\"]\r\n[Result \"*\"]\r\n[FEN \"8/7p/2pk1p1P/pp3Pp1/P2KP3/1P3P2/8/8 w - - 0 1\"]\r\n\r\n1. b4 axb4 2. a5\r\n

            int nCount = 0;

            for (int i = 0; i < pgnString.Length; i++)
            {
                if (pgnString[i] == 'n')
                    if (pgnString[i - 1] == '\\')
                        if(pgnString[i - 2] == 'r')
                            if(pgnString[i - 3] == '\\')
                                if (pgnString[i - 4] == 'n')
                                    if (pgnString[i - 5] == '\\')
                                        if (pgnString[i - 6] == 'r')
                                            if (pgnString[i - 7] == '\\')
                                                solution = pgnString.Substring((i + 1), pgnString.Length - (i + 1));

                //if(nCount == 4)
                //{
                //    solution = pgnString.Substring((i + 1), pgnString.Length - (i +1));
                //    break;
                //}
            }

            //#\r\n*"

            //[Result \"*\"]\r\n[FEN \"4r3/pp1n1pk1/1b3npp/3p2B1/1q6/1PN2QP1/P4P1P/3R1BK1 w - - 0 1\"]\r\n\r\n1.Bxf6+ Nxf6 2.Qxf6+ Kxf6 3.Nxd5+ Kg7 4.Nxb4 *"
            //[Result \"*\"]\r\n[FEN \"5k2/rr3pp1/3Pp2p/P2p4/2p5/6P1/R4PKP/1R6 w - - 0 1\"]\r\n\r\n1.Rxb7 Rxb7 2.a6 Ra7 3.Rb2 g5 4.Rb7 Rxa6 5.Rb8+ Kg7 6.d7 *"
            //[Date \"????.??.??\"]\r\n[FEN \"8/8/8/kP2R3/8/1r6/6P1/K7 w - - 0 1\"]\r\n\r\n1. g4 Ka4 2. g5 Ka3 3. Rc5 Rb4 4. Rc3+ Ka4 5. Rc2 Rg4 6. b6 Rg1+ 7. Ka2 Rxg5\r\n8. b7
            //[Event \"Ljubljana\"]\r\n[Site \"Ljubljana YUG\"]\r\n[Date \"1922.??.??\"]\r\n[Round \"?\"]\r\n[White \"Geza Maroczy\"]\r\n[Black \"Milan Vidmar\"]\r\n[Result \"1-0\"]\r\n[ECO \"C55\"]\r\n[PlyCount \"33\"]\r\n[FEN \"r4bkr/ppp3pp/2n1bq2/6N1/2pp2P1/8/PPP2P1P/R1BQR1K1 w - - 0 15\"]\r\n\r\n15.Rxe6 Qd8 16.Qf3 Qd7 17.Re7 1-0"

            //1.Rh1 Kh3(1... g2 2.Rxh2 + Kg3 3.Rh8 g1 = Q 4.Ne2 +)(1... Kg4 2.Ne2 Kf3 3.Nxg3 Kxg3 4.Nd3 Kg2 5.Nf2) 2.Nd3 Kg2(2... g2 3.Nf4 + Kg3 4.Nce2 + Kf3 5.Rxh2) 3.Nf4 + Kxh1 4.Nce2 g2 5.Ng3 + Kg1 6.Nh3#
            StringBuilder sb = new StringBuilder(solution);

            

            for(int i = solution.Length -1; i >= solution.Length - 6; i--)
            {
                Debug.Log(solution);
                if(solution[i] == '\\' || solution[i]  == 'n' || solution[i] == 'r' || solution[i] == '*' || solution[i] == '"')
                {
                    sb.Remove(i, 1);
                }
                    
            }

            solution = sb.ToString();

           


            
            return solution;
        }
    }
}
