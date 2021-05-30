import firebase from 'firebase/app';
import { useEffect } from 'react/cjs/react.development';

/*
    Listens for anys new chat messages and loads the most recent chat messages
*/
export const ChatListener = function(props) {
    
    const path = props.path;
    const onNewChatMessageCallback = props.onNewChatMessage;
    const onClearListenersCallback = props.onClearChatListeners;

    useEffect(()=> {

        if(!path){
            return;
        }

        const messageFetchLimit = parseInt(process.env.REACT_APP_CHAT_LAST_MESSAGES_COUNT) ?? 10;
        const chatMessagesRef = firebase.database().ref(path).limitToLast(messageFetchLimit);

        chatMessagesRef.on('child_added', (snapshot) => {
            const message = snapshot.val();

            if(onNewChatMessageCallback){
                onNewChatMessageCallback(message);
            }
        });

        return () => { 
            chatMessagesRef.off('child_added');

            if(onClearListenersCallback){
                onClearListenersCallback();
            }
        };
    }, [onNewChatMessageCallback, onClearListenersCallback, path]);

  return (null);
}
