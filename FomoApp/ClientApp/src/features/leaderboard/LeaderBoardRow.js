import './LeaderBoardRow.css';

import React from 'react';

export default function LeaderBoardRow(props) {

    function showLink() {
        if (props.link) {
            window.open(props.link, "_blank"); 
        }
    }
    
    return (
        <div className='leader-board-row' role="row" onClick={showLink}>
            <div className='leader-board-column leader-board-name' role="cell">{props.name}</div>
            <div className='leader-board-column leader-board-value' role="cell">{props.value}</div>
        </div>
    );
}