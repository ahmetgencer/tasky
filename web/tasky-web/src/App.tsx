import { Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Projects from './pages/Projects';
import ProjectBoard from './pages/ProjectBoard';
import ProtectedRoute from './components/ProtectedRoute';

export default function App() {
  return (
    <Routes>
      <Route path='/login' element={<Login />} />
      <Route
        path='/projects'
        element={
          <ProtectedRoute>
            <Projects />
          </ProtectedRoute>
        }
      />
      <Route
        path='/projects/:id'
        element={
          <ProtectedRoute>
            <ProjectBoard />
          </ProtectedRoute>
        }
      />
      <Route path='*' element={<Navigate to='/projects' />} />
    </Routes>
  );
}
