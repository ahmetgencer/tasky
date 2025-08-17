import { Navigate } from 'react-router-dom';
import React from 'react';
import { useAuthStore } from '../store/auth';

export default function ProtectedRoute({
  children,
}: {
  children: React.ReactNode;
}) {
  const token = useAuthStore((s) => s.token);
  if (!token) return <Navigate to='/login' replace />;
  return children;
}
