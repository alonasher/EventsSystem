import './App.css'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import AnalyzePage from './views/AnalyzePage/analyzePage'
import RecordingPage from './views/RecordingPage/recordingPage'
import Navbar from './components/Navbar/navbar';

function App() {

  return (
    <BrowserRouter>
      <Navbar />
      
      <Routes>
        <Route path="/" element={<RecordingPage />} />
        
        <Route path="/analyze" element={<AnalyzePage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App
