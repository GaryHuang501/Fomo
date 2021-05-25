import './Member.css';

import React from 'react';

/*
    User info that provides a portfolio link
*/
export const Member = function(props) {
  return (
    <div className='member standard-border large-border-radius' key={props.id} role='listitem'>
      <div className='member-content'>
        <span><i className="member-avatar fas fa-user-circle"></i></span><a href={`/portfolio/${props.id}`}>{props.name}</a>
      </div>
    </div>
  );
}