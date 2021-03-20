import './ChatMessage.css';
import '../../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/solid.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/regular.min.css';

import ChatStatusType from './ChatStatusType';
import React from 'react';

/*
    Represents an individual chat messages in the chat area.
*/
export const ChatMessage = function (props) {

    const chatStatusClass = props.status === ChatStatusType.SENT ? "chat-message-sent" : "chat-message-pending";

    return (
        <div role='log' className={'chat-message ' + chatStatusClass}>
            <div className='chat-message-info'>
                <span className='chat-user-info-name'>{props.userName}</span>
                <span className='chat-user-info-date-created'>{props.displayTime}</span>
                {props.status === ChatStatusType.ERROR ? <i className="fas fa-exclamation-circle chat-message-error-sent-icon" role='status'></i> : null}
            </div>
            <div className='chat-message-text'>{props.text}</div>
        </div>
    );
}
