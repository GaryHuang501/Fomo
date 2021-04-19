import './MembersList.css';

import { Member } from './Member';
import React from 'react';

/*
    Shows the list of given members
*/
export const MembersList = function(props) {

  const elements = props != null ? props.members.map(m => <Member key={m.id} id={m.id} name={m.name}></Member>) : [];

  return (
    <section className="members-list" role='list'>
        {elements}
    </section>
  );
}