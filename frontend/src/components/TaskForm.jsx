import { useState } from 'react'

function TaskForm({ onAdd }) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')

  async function handleSubmit(event) {
    event.preventDefault()
    const added = await onAdd({ title, description })
    if (added) {
      setTitle('')
      setDescription('')
    }
  }

  return (
    <form className="task-form" onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Task title (required)"
        value={title}
        maxLength={100}
        onChange={(e) => setTitle(e.target.value)}
      />
      <input
        type="text"
        placeholder="Description (optional)"
        value={description}
        maxLength={500}
        onChange={(e) => setDescription(e.target.value)}
      />
      <button type="submit">Add Task</button>
    </form>
  )
}

export default TaskForm
