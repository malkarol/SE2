import {
    VOTINGS_LOADING,
    VOTINGS_LOADED,
    VOTINGS_LOADING_ERROR
} from '../AppConstants/AppConstants'

const baseState = {
    votings: [],
    loading: false, 
    error: null,
}

export default function VotingsReducer(state = baseState, action) 
{
    switch(action.type) 
    {
        case VOTINGS_LOADED:
            return {
                ...state, 
                votings: action.payload, 
                loading: false, 
                error: null
            }

        case VOTINGS_LOADING:
            return {
                ...state, 
                loading: 
                action.payload, 
                error: null
            }

        case VOTINGS_LOADING_ERROR:
            alert('Error: ' + action.payload);
            return {
                ...state, 
                loading: false, 
                error: action.payload
            }
      
        default:
            return state;
  }
}