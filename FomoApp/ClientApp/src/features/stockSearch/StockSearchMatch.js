import React from 'react';
import './StockSearchMatch.css';

export const StockSearchMatch = function (props) {
  
  function clickCallback(){
    if(props.onClick){
      props.onClick(props.match.symbolId)
    }
  }

  return (
    <div className='stock-search-match' onClick={clickCallback}>
        <span className='stock-search-match-symbol'>{props.match.symbol}</span>
        <span className='stock-search-match-fullname' title={props.match.fullName}>{props.match.fullName}</span>
    </div>
  );
}
