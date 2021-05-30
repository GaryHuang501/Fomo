import './MembersPage.css';

import { getMemberData, selectMemberData } from './MembersSlice';
import { useDispatch, useSelector } from 'react-redux'

import FriendActivityBox from '../chatbox/FriendActivityBox'
import { MembersList } from './MembersList';
import { MembersListSeparator } from './MembersListSeparator';
import React from 'react';
import { useEffect } from 'react/cjs/react.development';

/*
    Shows the list of members links to the their portfolio pages.

    Members are grouped by their first letter.
*/
export default function MembersPage(){

  const dispatch = useDispatch();
  const memberData = useSelector(selectMemberData);
  const memberGroupings = memberData.memberGroupings;
  const uncategorizedMembers = memberData.uncategorizedMembers;
  
  useEffect(() => {
    dispatch(getMemberData());
  },[dispatch]);

  function AddMembersElementsToList(list, keyHeader, members){

    const validMembers = members.filter(m => m.name != null && m.name.length > 0);

    if(validMembers.length === 0) return;
    
    list.push(<MembersListSeparator key={`memberListSeparator_${keyHeader}`} header={keyHeader}></MembersListSeparator>);


    list.push(<MembersList key={`memberList_${keyHeader}`} members={validMembers}></MembersList>);
  }

  const elements = [];
  
  for(const groupingKey in memberGroupings){
    AddMembersElementsToList(elements, groupingKey, memberGroupings[groupingKey])
  }

  AddMembersElementsToList(elements, "Others", uncategorizedMembers);

  return (
    <main id="members-page">
      <div id="members-page-content">
        {elements}
      </div>
      <section id='member-page-friend-activity-box-container'>
          <h3 id='member-page-friend-activity-box-header'>Friend Activity</h3>
          <FriendActivityBox></FriendActivityBox>
      </section>
    </main>
  );
}