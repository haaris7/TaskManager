import { cn } from '../../utils/helpers';

const Input = ({ label, error, className, ...props }) => (
  <div className="space-y-1.5 w-full text-left">
    {label && <label className="text-sm font-semibold text-slate-600 ml-1">{label}</label>}
    <input
      className={cn(
        "w-full bg-white/60 border border-white/50 rounded-xl px-4 py-3 text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500/50 focus:border-sky-500/50 transition-all backdrop-blur-sm shadow-sm",
        error && "border-rose-500/50 focus:ring-rose-500/20 bg-rose-50/50",
        className
      )}
      {...props}
    />
  </div>
);

export default Input;