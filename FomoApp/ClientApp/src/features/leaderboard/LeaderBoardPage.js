import './LeaderBoardPage.css';

import { getLeaderBoardData, selectLeaderBoard } from './LeaderBoardSlice';
import { useDispatch, useSelector } from 'react-redux';

import LeaderBoard from './LeaderBoard';
import Options from './LeaderBoardOptions';
import React from 'react';
import { useEffect } from 'react/cjs/react.development';

export default function LeaderBoardPage() {

  const dispatch = useDispatch();
  const leaderBoard = useSelector(selectLeaderBoard);
  const mostBullish = leaderBoard.mostBullish;
  const mostBearish = leaderBoard.mostBearish;
  const bestPerformers = leaderBoard.bestPerformers;
  const worstPerformers = leaderBoard.worstPerformers;
  
  useEffect(() => {
    dispatch(getLeaderBoardData());
  },[dispatch]);

  return (
    <main id="leader-board-page">
      <div id="leader-board-container">
        <div className="leader-board-stocks">
          { mostBullish != null ? <LeaderBoard key={mostBullish.header} board={mostBullish} options={Options.mostBullish} ></LeaderBoard> : null }
          { mostBearish != null ? <LeaderBoard key={mostBearish.header} board={mostBearish} options={Options.mostBearish}></LeaderBoard> : null }
        </div>
        <div className="leader-board-performers">
          { bestPerformers != null ? <LeaderBoard key={bestPerformers.header} board={bestPerformers} options={Options.bestPerformers}></LeaderBoard> : null }
          { worstPerformers != null ? <LeaderBoard key={worstPerformers.header} board={worstPerformers} options={Options.worstPerformers}></LeaderBoard> : null }
        </div>
      </div>
    </main>
  );
}