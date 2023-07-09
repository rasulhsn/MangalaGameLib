using MangalaGameLib.Game.ObjectModel;
using System;
using System.Collections.Generic;

/*
    [SharpDevelop]
    Rasul Huseynov
    Lib Exchange : 2014.01.04
    No L : 2344012-444111-2885912348522
 */

namespace MangalaGameLib.Game
{
    public abstract class BaseGame
    {
        private Dictionary<Player, MangalaBoard<int>> _mainBoard;
        private Player _currentUser;
        private bool _finished;
        public event Action FinishedEvent;

        public bool FinishedSwitching
        {
            get { return this._finished; }
            private set
            {
                _finished = value;
                if(_finished == true)
                {
                    if( FinishedEvent != null)
                    {
                        FinishedEvent();
                    }
                }
            }
        }

        public Player GCurrentUser
        {
            get { return _currentUser; }
        }

        public MangalaBoard<int> User1
        {
            get
            {
                MangalaBoard<int> Cusr = new MangalaBoard<int>();
                bool result =_mainBoard.TryGetValue(Player.User1,out Cusr);
                return Cusr;
            }
        }

        public MangalaBoard<int> User2
        {
            get
            {
                MangalaBoard<int> Cusr = new MangalaBoard<int>();
                bool result = _mainBoard.TryGetValue(Player.User2, out Cusr);
                return Cusr;
            }
        }

        public BaseGame()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            this._currentUser = Player.User1;
            _mainBoard = new Dictionary<Player, MangalaBoard<int>>();
            _mainBoard.Add(Player.User1, new MangalaBoard<int>());
            _mainBoard.Add(Player.User2, new MangalaBoard<int>());
            for (int i = 0; i < 2; i++)
            {
                _mainBoard[(Player)i].Box = 0;
                for (int j = 0; j < 6; j++)
                {

                    _mainBoard[(Player)i].Board[j] = 4;
                }
            }
            FinishedSwitching = false;
        }

        public void CheckItFinish()
        {
            bool Switching = false;
            for (int j = 0; j < 6; j++)
            {
                if (_mainBoard[this._currentUser].Board[j] > 0)
                {
                    _finished = false;
                    Switching = true;
                    break;
                }
            }
            if (Switching == false)
            {
                TransferStoneUser(this._currentUser);
                this.FinishedSwitching = true;
            }
        }

        private void TransferStoneUser(Player FinishedUser)
        {
            MangalaBoard<int> TransferUser = OpponentSelection(FinishedUser);
            if (TransferUser != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    _mainBoard[FinishedUser].Box += TransferUser.Board[i];
                }
                for (int i = 0; i < 6; i++)
                {
                    _mainBoard[FinishedUser].Board[i] = 0;
                }
                return;
            }
            throw new Exception("Transfer user null or empty!");
        }

        public void MoveSequenceAuto(int IndexStone, Player LocalUsr)
        {       
            try
            {
                if (_finished == true)
                    throw new Exception("Game finished, Please restart game!");
    
                MangalaBoard<int> StartPlayer = _mainBoard[LocalUsr];

             
                if (StartPlayer.Board[IndexStone] > 0)
                {
            
                    int TempStone = StartPlayer.Board[IndexStone];
                    StartPlayer.Board[IndexStone] = 0; 
                  
                    if(IndexStone == 5 && TempStone >= 2)
                    {
                        
                        StartPlayer.Board[IndexStone] = 1;
                        TempStone -= 2;
                        StartPlayer.Box += 1;
                        if(TempStone >= 1)
                        {
                            Player OtherUser = GetOpponentSelection(this._currentUser);
                            MoveToOtherOpponent(_mainBoard[OtherUser], StartPlayer, ref TempStone);
                            this._currentUser = OtherUser;
                        }

                    }
                    else if (TempStone == 1)
                    {
                        OneStone(StartPlayer, IndexStone, ref TempStone);
                    }
                    else
                    {
                       
                        
                        for (int i = IndexStone; i < 6; i++)
                        {
                            StartPlayer.Board[i] += 1;
                            TempStone -= 1;

                            if (TempStone == 0)
                            {
                                this._currentUser = GetOpponentSelection(this._currentUser);
                                break;
                            }
                            else if (TempStone == 1)
                            {
                                OneStone(StartPlayer, i, ref TempStone);
                                break;
                            }
                            else if(i == 5 && TempStone > 1)
                            {
                                
                                TempStone -= 1;
                                StartPlayer.Box += 1;
                                Player OtherUser = GetOpponentSelection(this._currentUser);
                                MoveToOtherOpponent(_mainBoard[OtherUser], StartPlayer,ref TempStone);
                                this._currentUser = OtherUser;
                                break;
                            }
 
                        }

                    }
                }
                else
                {
                    throw new Exception("Stone not available!");
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void MoveToOtherOpponent(MangalaBoard<int> OpponentBoard, MangalaBoard<int> NowPlayer,ref int RemainingStone)
        {
            
            for (int i = 0; i < 6; i++)
            {
                if (RemainingStone == 0)
                    break;
                OpponentBoard.Board[i] += 1;
                RemainingStone -= 1;
                
                if(RemainingStone == 1 && i != 5)
                {
                    OpponentBoard.Board[i + 1] += 1;
                    RemainingStone -= 1;
                 
                    if ((OpponentBoard.Board[i + 1] % 2) == 0)
                    {
                        int OppenentStones = OpponentBoard.Board[i + 1];
                        OpponentBoard.Board[i + 1] = 0;
                        NowPlayer.Box += OppenentStones;
                        break;
                    }
                } 
                else if(i == 5 && RemainingStone >= 1)
                {
                    
                    RepetableLoop(NowPlayer, ref RemainingStone);
                    break;
                }
            }

        }

        private void RepetableLoop(MangalaBoard<int> NowPlayer,ref int RemainingStone)
        {
            if(RemainingStone == 1)
            {
                OneStone(NowPlayer, 0, ref RemainingStone);
            }
            else
            {
               
                for (int i = 0; i < 6; i++)
                {
                    NowPlayer.Board[i] += 1;
                    RemainingStone -= 1;

                    if (RemainingStone == 0)
                    {
                        this._currentUser = GetOpponentSelection(this._currentUser);
                        break;
                    }
                    else if (RemainingStone == 1)
                    {
                        OneStone(NowPlayer, i, ref RemainingStone);
                        break;
                    }
                    else if (i == 5 && RemainingStone > 1)
                    {
                        
                        RemainingStone -= 1;
                        NowPlayer.Box += 1;
                        Player OtherUser = GetOpponentSelection(this._currentUser);
                        MoveToOtherOpponent(_mainBoard[OtherUser], NowPlayer, ref RemainingStone);
                        this._currentUser = OtherUser;
                        break;
                    }

                }
            }
        }
        private void OneStone(MangalaBoard<int> NowPlayer,int IndexStone,ref int RemainingStone)
        {
     
            if (IndexStone == 5)
            {
                RemainingStone -= 1;
                NowPlayer.Box += 1;
            }
            else
            {
                
                RemainingStone -= 1;
                NowPlayer.Board[IndexStone + 1] += 1;
                
                if (NowPlayer.Board[IndexStone + 1] == 1)
                {
                    
                    int OpponentStone = TakeOtherStone(TwinsIndex(IndexStone + 1), OpponentSelection(this._currentUser));
                    if(OpponentStone > 0)
                    {
                        NowPlayer.Box += (OpponentStone + 1);
                        NowPlayer.Board[IndexStone + 1] = 0;
                    }
                }
                this._currentUser = GetOpponentSelection(this._currentUser);
            }
        }

        private int TwinsIndex(int Index)
        {
            if (Index == 0)
                return 5;
            else if (Index == 5)
                return 0;
            else if (Index == 1)
                return 4;
            else if (Index == 4)
                return 1;
            else if (Index == 2)
                return 3;
            else if (Index == 3)
                return 2;
            else
                throw new Exception("Oppenent value not available!");
        }
       
        private int TakeOtherStone(int OpponentIndex,MangalaBoard<int> OpponentBoard)
        {
            int ResultStones = OpponentBoard.Board[OpponentIndex];
            if(ResultStones > 0)
            {
                OpponentBoard.Board[OpponentIndex] = 0;
            }
            return ResultStones;
        }

        private Player GetOpponentSelection(Player User)
        {
            if (User == Player.User1)
                return Player.User2;
            else if (User == Player.User2)
                return Player.User1;
            else
                throw new Exception("Current user not defined!");
        }

        private void OppenentMoveSequence(Player CurrentUsr, int RemainingStone)
        {
            
            MangalaBoard<int> Opponent = OpponentSelection(CurrentUsr);
            if (Opponent != null)
            {
                for (int i = 5; i >= 0; i--)
                {
                    if (RemainingStone == 1)
                    {
                        Opponent.Board[i] += 1;
                        if ((Opponent.Board[i] % 2) == 0)
                        {
                            RemainingStone -= 1;
                            _mainBoard[(Player)CurrentUsr].Box += Opponent.Board[i];
                            Opponent.Board[i] = 0;
                            break;
                        }
                        else if (Opponent.Board[i] == 1)
                        {
                            RemainingStone -= 1;
                            _mainBoard[(Player)CurrentUsr].Box += Opponent.Board[i];
                            Opponent.Board[i] = 0;
                            break;
                        }
                        else
                        {
                            RemainingStone -= 1;
                            Opponent.Board[i] += 1;
                            this._currentUser = GetOpponentSelection(this._currentUser);
                            break;
                        }
                    }
                    else
                    {
                        Opponent.Board[i] += 1;
                        RemainingStone -= 1;
                    }
                }

                return;
            }
            throw new Exception("Opponent null or empty!");

        }

        private MangalaBoard<int> OpponentSelection(Player Usr)
        {
            if (Usr == Player.User1)
                return this._mainBoard[Player.User2];
            else if (Usr == Player.User2)
                return this._mainBoard[Player.User1];
            else
                return null;
        }
    }
}
