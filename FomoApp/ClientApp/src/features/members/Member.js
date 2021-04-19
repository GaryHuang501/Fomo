import './Member.css';

import { NavLink } from 'react-router-dom';
import React from 'react';

/*
    User info that provides a portfolio link
*/
export const Member = function(props) {
  return (
    <div className='member standard-border' key={props.id} role='listitem'>
      <div className='member-content'>
        <span><i className="member-avatar fas fa-user-circle"></i></span><a href={`/${props.id}`}>{props.name}</a>
      </div>
    </div>
  );
}