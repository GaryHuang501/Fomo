import { createSlice } from '@reduxjs/toolkit';

// Manages states for showing/hiding modals.
// Remark: Currently only profile modal uses this as it needs to display api errors. 
export const modalSlice = createSlice({
  name: 'modal',
  initialState: {
      showProfileModal: false
  },
  reducers: {
      showProfileModal: (state, action) => {
          state.showProfileModal = action.payload;
      }
  },
});

export default modalSlice.reducer;

export const { showProfileModal } = modalSlice.actions;

export const selectShowProfileModal = (state) => state.modal.showProfileModal;
