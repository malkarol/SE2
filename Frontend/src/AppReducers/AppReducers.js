import { combineReducers } from 'redux';
import VotingsReducer from './VotingsReducer';


const AppReducers = combineReducers({
    votings: VotingsReducer,
});

 export default AppReducers;