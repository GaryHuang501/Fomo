import firebase from 'firebase/app';
import { useEffect } from 'react/cjs/react.development';
import { userMessagesPath } from '../../app/FireBasePaths';

/*
    Listens for anys new chat messages and loads the most recent chat messages
*/
export const ChatListener = function(props) {
    
    const userId = props.userId;
    const onNewChatMessageCallback = props.onNewChatMessage;

    useEffect(()=> {

        if(!userId){
            return;
        }

        const messageFetchLimit = parseInt(process.env.REACT_APP_CHAT_LAST_MESSAGES_COUNT) ?? 10;
        const chatMessagesRef = firebase.database().ref(`${userMessagesPath}/${userId}`).limitToLast(messageFetchLimit);

        chatMessagesRef.on('child_added', (snapshot) => {
            const message = snapshot.val();

            if(onNewChatMessageCallback){
                onNewChatMessageCallback(message);
            }
        });

        return () => { 
            chatMessagesRef.off('child_added');
        };
    }, [onNewChatMessageCallback, userId]);

  return (null);
}
