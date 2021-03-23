import './VoteColumn.css';

import React, { useState } from 'react';

import { sendVote } from '../stocks/stocksSlice';
import { useDispatch } from 'react-redux';

/*
    Column in the portfolio stock row for the stock voting info.

    Will show upvote only if votes positive. If negative, only downvotes are shown.
    If votes are 0, nothing will be shown.

    If the user hovers over the stock row, it will be in edit mode, where they can upvote/downvote.
    Otherwise it will be display only / read only mode
*/
export default function VoteColumn(props){

    const VoteDirectionType = Object.freeze({ UPVOTE: 1, NONE: 0, DOWNVOTE: -1});
    
    // UseState so votes will reflect user's changes immediately and not wait for API dispatch.
    const [myVoteDir, setmyVoteDir] = useState(props.myVoteDir ?? 0);
    const [displayVotes, setDisplayVotes] = useState(props.votes ?? 0);
    
    const dispatch = useDispatch();

    let upVoteClasses = "fas fa-rocket fa-flip-horizontal";
    let downVoteClasses = "fas fa-rocket fa-flip-vertical"

    function onVote(e){
        const newVoteDirection = e.target.dataset.dir;

        // if the user clicks the same vote direction, it resets it to 0.
        let newActualDirection = myVoteDir == newVoteDirection ? VoteDirectionType.NONE : newVoteDirection;

        setmyVoteDir(newActualDirection);
        setDisplayVotes(v => v + (newActualDirection - myVoteDir));
        dispatch(sendVote({symbolId: props.symbolId, direction: newActualDirection}));
    }

    function setClassesForDisplayOnlyMode(){
        if(displayVotes > 0){
            upVoteClasses += " portfolio-stock-positive-delta";
            downVoteClasses += " hidden";
        }
        else if (displayVotes < 0){
            upVoteClasses += " hidden";
            downVoteClasses += " portfolio-stock-negative-delta";   
        }
    }

    function setClassesForEditOnlyMode(){
        if(myVoteDir > 0){
            upVoteClasses += " already-voted";
            downVoteClasses += " votable";
        }
        else if(myVoteDir < 0){
            downVoteClasses += " already-voted";
            upVoteClasses += " votable";
        }

        downVoteClasses += " votable";
        upVoteClasses += " votable";
    }

    if(props.isEditMode){
        setClassesForEditOnlyMode();
    }
    else{
        setClassesForDisplayOnlyMode();
    }

    let upvoteElement = <span><i className={upVoteClasses} data-dir={VoteDirectionType.UPVOTE} onClick={onVote} role="button"/></span>;
    let downVoteElement = <span><i className={downVoteClasses} data-dir={VoteDirectionType.DOWNVOTE} onClick={onVote} role="button"/></span>

    return (
        <td className="portfolio-column portfolio-row-votes" role='cell'>
        <div>
            {upvoteElement}
            <span className='portfolio-row-votes-value'> {displayVotes} </span>
            {downVoteElement}
        </div>
        </td>   
     );
}