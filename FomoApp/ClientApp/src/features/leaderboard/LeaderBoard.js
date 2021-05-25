import './LeaderBoard.css';

import LeaderBoardRow from './LeaderBoardRow';
import React from 'react';

export default function LeaderBoard(props) {
  let headerClasses;

  if(props.options){
      headerClasses = props.options.headerClasses;
  }

  function getLink(id, name){
    if(props.options && props.options.link){
      return props.options.link(id, name);
    }

    return null;
  }

  const listings = props.board.values.map( v =>
    <LeaderBoardRow key={props.board.header + v.id.toString()} name={v.name} value={v.value} link={getLink(v.id, v.name)}></LeaderBoardRow>
  );

  return (
    <article role='table' className="leader-board standard-border standard-border-radius">
        <div className={`leader-board-header ${headerClasses}`}>
          <h3>{props.board.header}</h3>
        </div>
        <div className="leader-board-column-headers" role="row">
          <h4 className="leader-board-column leader-board-name" role="columnheader" >{props.board.columnHeaderName}</h4>
          <h4 className="leader-board-column leader-board-value" role="columnheader" >{props.board.columnHeaderValue}</h4>
        </div>
        {listings}
    </article>
  );
}