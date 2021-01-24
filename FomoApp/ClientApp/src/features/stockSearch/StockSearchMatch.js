import './StockSearchMatch.css';

import React from 'react';

export const StockSearchMatch = function (props) {
  
  function clickCallback(){
    if(props.onClick){
      props.onClick(props.match.symbolId)
    }
  }

  return (
    <div className='stock-search-match' onClick={clickCallback} role='option'>
        <span className='stock-search-match-symbol'>{props.match.ticker}</span>
        <span className='stock-search-match-fullname' title={props.match.fullName}>{props.match.fullName}</span>
    </div>
  );
}
