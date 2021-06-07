import './FriendActivityBox.css';

import React, { useCallback } from 'react';
import { messageReceived, selectMessages, selectRefAlreadyExists } from './ChatSlice';
import { useDispatch, useSelector } from 'react-redux';

import { ChatListener } from './ChatListener';
import { ChatMessageArea } from './ChatMessageArea';
import { friendActivityPath } from '../../app/FireBasePaths';

/*
  Read only message box to listen to history activity that uses chat components.
*/
export default function FriendActivityBox(){

  const dispatch = useDispatch();
  const path = friendActivityPath;
  const refAlreadyExist = useSelector(state => selectRefAlreadyExists(state, path));
  const activityMessages = useSelector(state => selectMessages(state, path));
  const onNewChatMessage = useCallback( message => { dispatch(messageReceived({path: path, message: message})); }, [dispatch, path]);

  return (
    <aside id="friend-activity-box">
      <div id="chatbox-top-filler-box"></div> {/* Empty area to give spacing between scrollbar and rounded bordders*/}
      <ChatListener 
        path = {path} 
        onNewChatMessage={onNewChatMessage}
        bindNewListener={!refAlreadyExist}
        aria-level="1" role="heading">
      </ChatListener>
      <ChatMessageArea 
        chatMessages={activityMessages}>          
      </ChatMessageArea>
      <div id="chatbox-bottom-filler-box"></div> {/* Empty area to give spacing between scrollbar and rounded bordders*/}
    </aside>
  );
}