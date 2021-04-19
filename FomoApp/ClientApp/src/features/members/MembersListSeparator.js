import './MembersListSeparator.css';

import React from 'react';

/*
    Header to separate each member list grouping.
*/
export const MembersListSeparator = function(props) {

  return (
    <h3 className='member-list-separator'>
        {props.header}
    </h3>
  );
}