import axios from 'axios';

const API_BASE_URL = 'http://localhost:5206/api'; 

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor to add token to every request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor to handle global errors (like 401 Unauthorized)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.clear();
      window.location.href = '/login'; // Force redirect
    }
    return Promise.reject(error);
  }
);

export const authService = {
  login: async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    return response.data;
  },
};

export const taskService = {
  getAllTasks: async () => (await api.get('/task')).data,
  createTask: async (taskData) => (await api.post('/task', taskData)).data,
  changeTaskStatus: async (taskId, status) => (await api.post(`/task/${taskId}/status/${status}`)).data,
  updateTask: async (taskId, updateTaskDto) => (await api.put(`/task/${taskId}`, updateTaskDto)).data,
  deleteTask: async (taskId) => (await api.delete(`/task/${taskId}`)).data,
};

export const userService = {
    getAllUsers: async () => (await api.get('/user')).data,
    createUser: async (userData) => (await api.post('/user', userData)).data,
    updateUser: async (userId, userData) => (await api.put(`/user/${userId}`, userData)).data,
    deleteUser: async (userId) => (await api.delete(`/user/${userId}`)).data,
};

export default api;