import React, { useState } from 'react';
import { selectMyUser, selectUpdateError, selectUpdateStatus, updateAccount } from '../../features/login/LoginSlice';
import { selectShowProfileModal, showProfileModal } from './ModalSlice';
import { useDispatch, useSelector } from 'react-redux'

import { LoadingOverlay } from '../loading/LoadingOverlay';
import Modal from './Modal';
import ProfileSettings from '../../features/login/ProfileSettings';
import { useEffect } from 'react';

// Modal showing the profile settings
export default function ProfileModal(props) {

    const dispatch = useDispatch();
    const myUser = useSelector(selectMyUser);
    const updateStatus = useSelector(selectUpdateStatus);
    const apiErrors = useSelector(selectUpdateError);
    const showModal = useSelector(selectShowProfileModal);
    const [submitted, setSubmitted] = useState(false);

    function onUpdateUser(user) {
        if (updateStatus !== 'loading') {
            dispatch(updateAccount(user));
        }
    }

    useEffect(() => {
        if(updateStatus === 'loading'){
            setSubmitted(true);
        }
        else if (updateStatus === 'succeeded' && submitted){
            dispatch(showProfileModal(false));
            setSubmitted(false);
        }
        else if(updateStatus === 'failed')
        {
            setSubmitted(false);
        }
    }, [updateStatus, submitted, dispatch]);

    return (
        <React.Fragment>
            {showModal
                ?
                <div>
                 { submitted ? <LoadingOverlay/> : null }
                 <Modal onCancel={props.onClose}>            
                    <ProfileSettings heading={myUser.name} onSubmit={onUpdateUser} myUser={myUser} apiErrors={apiErrors}/>
                  </Modal>
                  </div>
                : null
            }
        </React.Fragment>
    );
}
